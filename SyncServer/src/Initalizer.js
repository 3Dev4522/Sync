
import fs       from 'fs';
import path     from 'path';
import chokidar from 'chokidar';
import archiver from 'archiver';
import amqp     from 'amqplib';
import log      from './log/logger.js';
import { 
    BROKER_ID,
    BROKER_IP,
    BROKER_PORT,
    BROKER_PW,
    POLLING,
    REPO,
} from './appConfig.js';

let channel;
let connection;

export default async function Initalizer () {
    try {
        connection = await amqp.connect(`amqp://${BROKER_ID}:${BROKER_PW}@${BROKER_IP}:${BROKER_PORT}`);
    }
    catch (err) {
        log.error('cant connect to broker');
        return;
    }

    log.info('connected to broker');
    channel = await connection.createConfirmChannel();
    initFileWatcher(REPO + "\\");
}

function initFileWatcher (repositoryPath) {
    if (!fs.existsSync(repositoryPath))              fs.mkdirSync(repositoryPath);
    if (!fs.existsSync(repositoryPath + "/.cache"))  fs.mkdirSync(repositoryPath + "/.cache");

    log.info(`watcher=${repositoryPath}`);
    const watcher = chokidar.watch(repositoryPath, {
        ignoreInitial: true,        
        depth: 0,
    });

    watcher.on('addDir', async (filePath) => {
        const innerWatcher = chokidar.watch(filePath);
        let timer = null;
        
        innerWatcher.on('all', async () => {
            clearTimeout(timer);
            timer = setTimeout(() => {
                createZip(repositoryPath, filePath);
                innerWatcher.close();
            }, POLLING);
        });
    });
}

function createZip (repositoryPath, dirPath) {
    const cacheDir = repositoryPath + "/.cache";

    if (!fs.existsSync(cacheDir)) {
        fs.mkdirSync(cacheDir);
        fs.chmodSync(cacheDir, fs.statSync(cacheDir).mode | 0o4000);
    }
    
    const output  = fs.createWriteStream(path.join(cacheDir, path.basename(dirPath) + '.zip'));
    const archive = archiver('zip', { zlib: { level: 9 } });

    archive.pipe(output);
    archive.directory(dirPath, false);
    archive.finalize();

    output.on('close', () => publish(path.basename(dirPath)));
}

function publish (data) {
    return new Promise((resolve, reject) => {
        channel.publish(
            "update", 
            "", 
            Buffer.from(JSON.stringify(data), 'utf-8'),
            { persistent: true },
            (err) => {
                if (err)
                    return reject(err);

                log.info(`publish=${data}`);
                resolve();
            }
        );
    });
}
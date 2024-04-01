
import express           from 'express';
import Initalizer        from './Initalizer.js';
import { REPO, print }   from './appConfig.js';
import logger from './log/logger.js';

const app = express();

app.listen(8282, () => { 
    print();
    Initalizer(); 
}); 

app.get('/update/:version', async (request, response) => {
    const { version } = request.params;
    const ip = request.header["x-forwarded-for"] || request.connection.remoteAddress;
    
    logger.info(`${version} download=`, ip);
    response.setHeader('Content-Disposition', `attachment; filename=${version + ".zip"}`);
    response.sendFile(`${REPO}/.cache/${version}.zip`);
});
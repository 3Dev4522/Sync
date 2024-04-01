
import winston, { createLogger } from "winston";
import daily   from 'winston-daily-rotate-file';
import appRoot from 'app-root-path';

const LOG_DIR = `${appRoot}/logs`;
const { combine, timestamp, label, printf } = winston.format;

const logFormat = printf(({level, message, label, timestamp }) => {
    return `${timestamp} [${label}] ${level}: ${message}`;
});

// FILE LOG
const logger = createLogger({
    format: combine(label({label: "SYNC_SERVER"}), timestamp(), logFormat),
    transports: [
        new daily({
            level: "info",
            datePattern: "YYYY-MM-DD",
            dirname: LOG_DIR,
            filename: "sync-%DATE%.log",
            maxSize: "20m",
            maxFiles: "30d",
        }),
        new daily({
            level: "error",
            datePattern: "YYYY-MM-DD",
            dirname: LOG_DIR,
            filename: "sync-%DATE%.error.log",
            maxSize: "20m",
            maxFiles: "30d",
        }),
    ]
});

// DEVELOP (CONSOLE LOG)
logger.add(
    new winston.transports.Console({
        format: winston.format.combine(
            winston.format.colorize(),
            winston.format.simple()
        )
    })
);

export default logger;



import dotenv from 'dotenv';
dotenv.config();

export const BROKER_IP   = process.env.bip;
export const BROKER_PORT = process.env.bpt;
export const BROKER_ID   = process.env.bid;
export const BROKER_PW   = process.env.bpw;
export const REPO        = process.env.repo;
export const POLLING     = process.env.polling;

export function print () {
    const settings = `
            [CURRENT-SETTING]
        BROKER_IP:      ${BROKER_IP}
        BROKER_PORT:    ${BROKER_PORT}
        BROKER_ID:      ${BROKER_ID}
        BROKER_PW:      ${BROKER_PW}
        REPOSITORY:     ${REPO}
        POLLING:        ${POLLING}
    `;

    console.log(settings);
}
const jwt = require('jsonwebtoken');
const AWS = require('aws-sdk');
const axios = require('axios');

const secretsManager = new AWS.SecretsManager();
let cachedSecret;

async function getJwtSecret() {
    if (cachedSecret) return cachedSecret;

    const secretId = process.env.JWT_SECRET_ID || 'sr360/jwt-secret';
    const secretValue = await secretsManager.getSecretValue({ SecretId: secretId }).promise();

    if (!secretValue.SecretString) {
        await pushToLoki('error', 'JWT secret is empty or not in SecretString format', { secretId });
        throw new Error('JWT secret missing');
    }

    const parsed = JSON.parse(secretValue.SecretString);
    if (!parsed.JWT_SECRET) {
        await pushToLoki('error', 'JWT_SECRET not found in secret', { secretId });
        throw new Error('JWT_SECRET not found');
    }

    cachedSecret = parsed.JWT_SECRET;
    return cachedSecret;
}

async function pushToLoki(level, message, additional = {}, context = {}) {
    const payload = {
        streams: [
            {
                stream: {
                    job: 'lambda-auth',
                    level: level || 'info',
                    tenant_id: context.tenant_id || 'unknown'
                },
                values: [
                    [
                        `${Date.now()}000000`,
                        JSON.stringify({
                            level,
                            message,
                            ...context,
                            ...additional,
                            timestamp: new Date().toISOString()
                        })
                    ]
                ]
            }
        ]
    };

    try {
        await axios.post(process.env.LOKI_URL, payload, {
            auth: {
                username: process.env.LOKI_USER,
                password: process.env.LOKI_PASSWORD
            },
            headers: { 'Content-Type': 'application/json' },
            timeout: 2000
        });
    } catch (err) {
        console.error('Failed to push log to Loki:', err.message);
    }
}

exports.handler = async (event) => {
    const requestLogContext = {};

    await pushToLoki('info', 'Incoming event', { event }, requestLogContext);

    try {
        const authHeader = event.headers?.authorization || event.headers?.Authorization;

        if (!authHeader || !authHeader.startsWith('Bearer ')) {
            await pushToLoki('warn', 'Missing or malformed Authorization header', {
                header: authHeader
            }, requestLogContext);
            return {
                isAuthorized: false,
            };
        }

        const token = authHeader.replace('Bearer ', '');
        const JWT_SECRET = await getJwtSecret();

        const decoded = jwt.verify(token, JWT_SECRET);
        
        Object.assign(requestLogContext, {
            sub: String(decoded.sub),
            email: String(decoded.email),
            user_name: String(decoded.user_name),
            tenant_id: String(decoded.tenant_id),
            role_id: String(decoded.role_id),
            locale: String(decoded.locale),
            trace_id: "trace-20240509-testslug",
            env: String(decoded.env),
            jti: String(decoded.jti),
            iat: String(decoded.iat)
        });

        await pushToLoki('info', 'JWT verified', {}, requestLogContext);

        const { locale, ...contextWithoutLocale } = requestLogContext;

        return {
            isAuthorized: true,
            context: contextWithoutLocale
        };
    } catch (err) {
        let reason = 'UNKNOWN';
        if (err.name === 'TokenExpiredError') reason = 'TOKEN_EXPIRED';
        else if (err.name === 'JsonWebTokenError') reason = 'INVALID_TOKEN';

        await pushToLoki('error', 'JWT verification failed', {
            error: err.message,
            name: err.name,
            stack: err.stack,
            reason
        }, requestLogContext);

        return {
            isAuthorized: false
        };
    }
};
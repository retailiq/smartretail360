'use strict';
const { v4: uuidv4 } = require('uuid');

exports.handler = (event, context, callback) => {
    const request = event.Records[0].cf.request;
    const headers = request.headers;

    const traceId = uuidv4();

    headers['x-trace-id'] = [{
        key: 'X-Trace-Id',
        value: traceId
    }];

    console.log('[InjectTraceIdLambdaEdge] Injected trace ID:', traceId);
    callback(null, request);
};
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using SmartRetail360.Messaging.Interfaces;

namespace SmartRetail360.Messaging.Services;

public class SqsEmailProducer : IEmailQueueProducer
{
    private readonly IConfiguration _config;
    private readonly IAmazonSQS _sqs;

    public SqsEmailProducer(IConfiguration config)
    {
        _config = config;
        _sqs = new AmazonSQSClient(
            _config["AWS:AccessKey"],
            _config["AWS:SecretKey"],
            new AmazonSQSConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.APSoutheast2
            });
    }

    public async Task SendAsync(object message)
    {
        var queueUrl = (await _sqs.GetQueueUrlAsync(_config["Sqs:QueueName"])).QueueUrl;
        var body = JsonSerializer.Serialize(message);
        await _sqs.SendMessageAsync(new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = body
        });
    }
}
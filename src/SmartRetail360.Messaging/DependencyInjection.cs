using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartRetail360.Messaging.Interfaces;
using SmartRetail360.Messaging.Services;

namespace SmartRetail360.Messaging;

public static class DependencyInjection
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IAmazonSQS>(
            new AmazonSQSClient(
                config["AWS:AccessKey"],
                config["AWS:SecretKey"],
                new AmazonSQSConfig
                {
                    RegionEndpoint = Amazon.RegionEndpoint.APSoutheast2
                }));
        services.AddSingleton<IEmailQueueProducer, SqsEmailProducer>();
        return services;
    }
}
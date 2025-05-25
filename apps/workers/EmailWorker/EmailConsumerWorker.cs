using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.Common.UserContext;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Infrastructure.Services.Notifications.Configuration;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Messaging.Payloads;
using System.Text.Json;
using SmartRetail360.Shared.Context;

namespace EmailWorker;

public class EmailConsumerWorker : BackgroundService
{
    private readonly ILogger<EmailConsumerWorker> _logger;
    private readonly IConfiguration _config;
    private readonly IAmazonSQS _sqs;
    private readonly IServiceScopeFactory _scopeFactory;

    public EmailConsumerWorker(
        ILogger<EmailConsumerWorker> logger,
        IConfiguration config,
        IAmazonSQS sqs,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _config = config;
        _sqs = sqs;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueUrl = (await _sqs.GetQueueUrlAsync(_config["Sqs:QueueName"], stoppingToken)).QueueUrl;
        _logger.LogInformation("EmailConsumerWorker started. Listening to queue: {QueueUrl}", queueUrl);

        var receiveRequest = new ReceiveMessageRequest
        {
            QueueUrl = queueUrl,
            MaxNumberOfMessages = 1,
            WaitTimeSeconds = 10,
            MessageSystemAttributeNames = ["All"],
            MessageAttributeNames = ["All"],
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            ReceiveMessageResponse? response;

            try
            {
                response = await _sqs.ReceiveMessageAsync(receiveRequest, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to receive message from SQS.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                continue;
            }

            if (response?.Messages == null || response.Messages.Count == 0)
                continue;

            foreach (var message in response.Messages)
            {
                _logger.LogInformation("[RECEIVED] MessageId: {MessageId}, BodySize: {Size}",
                    message.MessageId,
                    message.Body?.Length ?? 0);

                var success = await HandleMessageAsync(message);

                if (success)
                {
                    await _sqs.DeleteMessageAsync(queueUrl, message.ReceiptHandle, stoppingToken);
                }
            }
        }
    }

    private async Task<bool> HandleMessageAsync(Message message)
    {
        if (string.IsNullOrWhiteSpace(message.Body))
        {
            _logger.LogWarning("Empty message body. Skipping. MessageId: {MessageId}", message.MessageId);
            return false;
        }

        ActivationEmailPayload? payload;
        try
        {
            payload = JsonSerializer.Deserialize<ActivationEmailPayload>(message.Body);
            if (payload == null)
            {
                _logger.LogWarning("Deserialization failed. Invalid body. MessageId: {MessageId}", message.MessageId);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse message JSON. MessageId: {MessageId}", message.MessageId);
            return false;
        }

        using var scope = _scopeFactory.CreateScope();
        var userContext = scope.ServiceProvider.GetRequiredService<IUserContextService>();
        var emailContext = scope.ServiceProvider.GetRequiredService<EmailContext>();
        var safeExecutor = scope.ServiceProvider.GetRequiredService<ISafeExecutor>();
        var dispatcher = scope.ServiceProvider.GetRequiredService<ILogDispatcher>();

        userContext.Inject(new UserExecutionContext
        {
            TenantId = payload.TenantId,
            TraceId = payload.TraceId,
            Locale = payload.Locale,
            Email = payload.Email,
            UserId = payload.UserId,
            RoleId = payload.RoleId,
            Module = LogSourceModules.EmailWorker,
            IpAddress = payload.IpAddress,
            Action = payload.Action,
            RoleName = payload.RoleName,
            LogId = payload.LogId
        });

        var variables = new Dictionary<string, string>
        {
            ["traceId"] = payload.TraceId,
            ["locale"] = payload.Locale,
            ["token"] = payload.Token,
            ["timestamp"] = payload.Timestamp,
            ["userName"] = payload.UserName,
            ["emailValidationMinutes"] = payload.EmailValidationMinutes,
        };
        
        var result = await safeExecutor.ExecuteAsync(
            async () =>
            {
                await CultureScope.RunWithCultureAsync(payload.Locale, async () =>
                {
                    await emailContext.SendAsync(payload.EmailTemplate, payload.Email, variables);
                });
            },
            LogEventType.EmailSendFailure,
            LogReasons.EmailSendFailed,
            ErrorCodes.EmailSendFailed
        );

        if (result.IsSuccess)
        {
            await dispatcher.Dispatch(LogEventType.EmailSendSuccess);
        }

        return result.IsSuccess;
    }
}
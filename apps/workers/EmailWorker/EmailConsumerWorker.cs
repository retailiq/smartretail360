using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SmartRetail360.Infrastructure.Services.Notifications.Configuration;
using SmartRetail360.Shared.Enums;
using System.Text.Json;
using SmartRetail360.Application.Common;
using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.Common.UserContext;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Shared.DTOs.Messaging;

public class EmailConsumerWorker : BackgroundService
{
    private readonly ILogger<EmailConsumerWorker> _logger;
    private readonly IConfiguration _config;
    private readonly IAmazonSQS _sqs;
    private readonly EmailContext _emailContext;
    private readonly ISafeExecutor _safeExecutor;
    private readonly IUserContextService _userContext;
    private readonly ILogDispatcher _dispatcher;

    public EmailConsumerWorker(
        ILogger<EmailConsumerWorker> logger,
        IConfiguration config,
        IAmazonSQS sqs,
        EmailContext emailContext,
        ISafeExecutor safeExecutor,
        IUserContextService userContext,
        ILogDispatcher dispatcher)
    {
        _logger = logger;
        _config = config;
        _sqs = sqs;
        _emailContext = emailContext;
        _safeExecutor = safeExecutor;
        _userContext = userContext;
        _dispatcher = dispatcher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueUrl = (await _sqs.GetQueueUrlAsync(_config["Sqs:QueueName"])).QueueUrl;
        _logger.LogInformation("EmailConsumerWorker started. Listening to queue: {QueueUrl}", queueUrl);

        var receiveRequest = new ReceiveMessageRequest
        {
            QueueUrl = queueUrl,
            MaxNumberOfMessages = 1,
            WaitTimeSeconds = 10,
            MessageSystemAttributeNames = new List<string> { "All" },
            MessageAttributeNames = new List<string> { "All" }
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
            {
                continue;
            }

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

            _userContext.Inject(
                tenantId: payload.TenantId,
                traceId: payload.TraceId,
                locale: payload.Locale,
                clientEmail: payload.Email,
                userId: payload.UserId,
                roleId: payload.RoleId,
                module: LogSourceModules.EmailWorker,
                accountType: payload.AccountType,
                ipAddress: payload.IpAddress,
                action: payload.Action
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse message JSON. MessageId: {MessageId}", message.MessageId);
            return false;
        }

        var traceId = payload.TraceId ?? Guid.NewGuid().ToString("N");

        var variables = new Dictionary<string, string>
        {
            ["traceId"] = traceId,
            ["tenantId"] = payload.TenantId.ToString(),
            ["locale"] = payload.Locale ?? "en",
            ["token"] = payload.Token ?? string.Empty,
            ["timestamp"] = payload.Timestamp ?? DateTime.UtcNow.ToString("o")
        };

        var result = await _safeExecutor.ExecuteAsync(
            async () =>
            {
                await CultureScope.RunWithCultureAsync(payload.Locale,
                    async () =>
                    {
                        await _emailContext.SendAsync(EmailTemplate.TenantAccountActivation, payload.Email!, variables);
                    });
            },
            LogEventType.EmailSendFailure,
            LogReasons.EmailSendFailed,
            ErrorCodes.EmailSendFailed
        );
        
        if (result.IsSuccess)
        {
            await _dispatcher.Dispatch(LogEventType.EmailSendSuccess);
        }
        
        return result.IsSuccess;
    }
}
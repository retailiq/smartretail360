using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SmartRetail360.Infrastructure.DTOs.Messaging;
using SmartRetail360.Infrastructure.Services.Notifications.Configuration;
using SmartRetail360.Shared.Enums;
using System.Text.Json;
using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Application.Common;

public class EmailConsumerWorker : BackgroundService
{
    private readonly ILogger<EmailConsumerWorker> _logger;
    private readonly IConfiguration _config;
    private readonly IAmazonSQS _sqs;
    private readonly EmailContext _emailContext;
    private readonly ISafeExecutor _safeExecutor;
    private readonly IUserContextService _userContext;

    public EmailConsumerWorker(
        ILogger<EmailConsumerWorker> logger,
        IConfiguration config,
        IAmazonSQS sqs,
        EmailContext emailContext,
        ISafeExecutor safeExecutor,
        IUserContextService userContext)
    {
        _logger = logger;
        _config = config;
        _sqs = sqs;
        _emailContext = emailContext;
        _safeExecutor = safeExecutor;
        _userContext = userContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueUrl = (await _sqs.GetQueueUrlAsync(_config["Sqs:QueueName"])).QueueUrl;
        _logger.LogInformation("启动 EmailConsumerWorker，绑定队列：{QueueUrl}", queueUrl);

        while (!stoppingToken.IsCancellationRequested)
        {
            ReceiveMessageResponse? response;

            try
            {
                response = await _sqs.ReceiveMessageAsync(new ReceiveMessageRequest
                {
                    QueueUrl = queueUrl,
                    MaxNumberOfMessages = 5,
                    WaitTimeSeconds = 10,
                    AttributeNames = new List<string> { "All" }
                }, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "从 SQS 拉取消息失败");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                continue;
            }

            if (response?.Messages == null || response.Messages.Count == 0)
            {
                Console.WriteLine("本轮无消息");
                continue;
            }

            foreach (var message in response.Messages)
            {
                await HandleMessageAsync(queueUrl, message);
            }
        }
    }

    private async Task HandleMessageAsync(string queueUrl, Message message)
    {
        if (string.IsNullOrWhiteSpace(message.Body))
        {
            _logger.LogWarning("收到空消息体，跳过。MessageId: {MessageId}", message.MessageId);
            return;
        }

        ActivationEmailPayload? payload;
        try
        {
            payload = JsonSerializer.Deserialize<ActivationEmailPayload>(message.Body);
            if (payload == null)
            {
                _logger.LogError("反序列化失败，消息内容为：{Body}", message.Body);
                return;
            }

            _userContext.Inject(
                tenantId: payload.TenantId,
                traceId: payload.TraceId,
                locale: payload.Locale,
                clientEmail: payload.Email,
                userId: payload.UserId,
                roleId: payload.RoleId,
                module: payload.Module,
                accountType: payload.AccountType
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "解析消息 JSON 出错，跳过该条：{Body}", message.Body);
            return;
        }

        try
        {
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
                    await CultureScope.RunWithCultureAsync(payload.Locale, async () =>
                    {
                        await _emailContext.SendAsync(EmailTemplate.TenantAccountActivation, payload.Email!, variables);
                    });
                },
                LogEventType.EmailSendFailure,
                LogReasons.EmailSendFailed,
                ErrorCodes.EmailSendFailed
            );

            if (!result.IsSuccess)
            {
                _logger.LogError("发送邮件失败，错误码：{ErrorCode}，TraceId: {TraceId}", ErrorCodes.EmailSendFailed, traceId);
                // throw new Exception($"邮件发送失败：{traceId}");
            }

            await _sqs.DeleteMessageAsync(queueUrl, message.ReceiptHandle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送邮件失败，未删除消息：{Body}", message.Body);
        }
    }
}
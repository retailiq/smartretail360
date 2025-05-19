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
        var workerId = _config["Worker:Id"]; 
        var queueUrl = (await _sqs.GetQueueUrlAsync(_config["Sqs:QueueName"])).QueueUrl;
        _logger.LogInformation("启动 EmailConsumerWorker，绑定队列：{QueueUrl}", queueUrl);

        while (!stoppingToken.IsCancellationRequested)
        {
            ReceiveMessageResponse? response = null;

            try
            {
                response = await _sqs.ReceiveMessageAsync(new ReceiveMessageRequest
                {
                    QueueUrl = queueUrl,
                    MaxNumberOfMessages = 5,
                    WaitTimeSeconds = 10
                }, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "从 SQS 拉取消息失败，跳过本轮");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                continue;
            }

            if (response?.Messages == null || response.Messages.Count == 0)
            {
                _logger.LogInformation("本轮无消息");
                continue;
            }

            _logger.LogInformation("本轮拉取消息数：{Count}", response.Messages.Count);

            foreach (var message in response.Messages)
            {
                if (string.IsNullOrWhiteSpace(message.Body))
                {
                    _logger.LogWarning("收到空消息体，跳过。MessageId: {MessageId}", message.MessageId);
                    continue;
                }

                ActivationEmailPayload? payload = null;
                

                try
                {
                    payload = JsonSerializer.Deserialize<ActivationEmailPayload>(message.Body);
                    if (payload == null)
                    {
                        _logger.LogError("反序列化失败，消息内容为：{Body}", message.Body);
                        continue;
                    }
                    
                    
                    _userContext.TenantId = payload.TenantId;
                    _userContext.TraceId = payload.TraceId;
                    _userContext.Locale = payload.Locale;
                    _userContext.ClientEmail = payload.Email;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "解析消息 JSON 出错，跳过该条：{Body}", message.Body);
                    continue;
                }

                try
                {
                    var email = payload.Email ?? string.Empty;
                    var traceId = payload.TraceId ?? Guid.NewGuid().ToString("N");
                    _logger.LogInformation("准备发送邮件至：{Email} TraceId: {TraceId}", email, traceId);

                    var variables = new Dictionary<string, string>
                    {
                        ["traceId"] = traceId,
                        ["tenantId"] = payload.TenantId.ToString(),
                        ["locale"] = payload.Locale ?? "en",
                        ["token"] = payload.Token ?? string.Empty,
                        ["timestamp"] = payload.Timestamp ?? DateTime.UtcNow.ToString("o")
                    };
                    
                    Console.WriteLine($"Tetant ID: {payload.TenantId}");
                    
                    await _safeExecutor.ExecuteAsync(
                        async () =>
                        {
                            await CultureScope.RunWithCultureAsync(payload.Locale, async () =>
                            {
                                _logger.LogInformation("即将调用 EmailContext.SendAsync...");
                                await _emailContext.SendAsync(EmailTemplate.TenantAccountActivation, payload.Email!, variables);
                                _logger.LogInformation("EmailContext.SendAsync 调用完毕");
                            });
                        },
                        LogEventType.EmailSendFailure,
                        LogReasons.EmailSendFailed,
                        ErrorCodes.EmailSendFailed,
                        email: payload?.Email,
                        tenantId: payload?.TenantId
                    );
                    

                    await _sqs.DeleteMessageAsync(queueUrl, message.ReceiptHandle);
                    _logger.LogInformation("邮件发送成功，消息已删除：{MessageId}", message.MessageId);
                }
                catch (Exception ex)
                {
                    
                    _logger.LogError(ex, "发送邮件失败，未删除消息：{Body}", message.Body);
                    // 不删，让 SQS 自动重试 & DLQ 接管
                }
            }
        }
    }
}
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Application.Interfaces.Messaging;
using SmartRetail360.Application.Interfaces.Redis;
using SmartRetail360.Infrastructure.Common.DependencyInjection;

namespace SmartRetail360.Infrastructure.Services.Notifications.Models;

public class NotificationDependencies : BaseDependencies
{
    public IRedisLimiterService RedisLimiterService { get; set; }
    public IEmailQueueProducer EmailQueueProducer { get; set; }
    public IAccountSupportService AccountSupport { get; set; }
}
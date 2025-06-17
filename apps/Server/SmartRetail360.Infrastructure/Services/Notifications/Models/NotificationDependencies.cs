using SmartRetail360.Caching.Interfaces;
using SmartRetail360.Infrastructure.Common.DependencyInjection;
using SmartRetail360.Messaging.Interfaces;
using SmartRetail360.Platform.Interfaces;

namespace SmartRetail360.Infrastructure.Services.Notifications.Models;

public class NotificationDependencies : BaseDependencies
{
    public IRedisLimiterService RedisLimiterService { get; set; }
    public IEmailQueueProducer EmailQueueProducer { get; set; }
    public IAccountSupportService AccountSupport { get; set; }
}
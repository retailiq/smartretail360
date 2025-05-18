using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Application.Interfaces.Services;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Infrastructure.Services.Messaging;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Options;

namespace SmartRetail360.Infrastructure.Services.Notifications.Models;

public class NotificationDependencies
{
    public required AppDbContext Db { get; init; }
    public required AppOptions AppOptions { get; init; }
    public required IUserContextService UserContext { get; init; }
    public required MessageLocalizer Localizer { get; init; }
    public required IRedisLimiterService RedisLimiterService { get; init; }
    public required ILogDispatcher LogDispatcher { get; init; }
    public required SqsEmailProducer EmailQueueProducer { get; init; }
    public required ISafeExecutor SafeExecutor { get; init; }
}
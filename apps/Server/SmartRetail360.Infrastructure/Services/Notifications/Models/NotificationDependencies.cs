using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.Common.UserContext;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Application.Interfaces.Redis;
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
    public required IGuardChecker GuardChecker { get; init; }
    public required IRedisOperationService RedisOperation { get; init; }
    public required IPlatformContextService PlatformContext { get; init; }
    public required IAccountSupportService AccountSupport { get; init; }
}
using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.Common.UserContext;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Application.Interfaces.Redis;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Infrastructure.Services.Messaging;
using SmartRetail360.Infrastructure.Services.Notifications.Configuration;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Options;

namespace SmartRetail360.Infrastructure.Common.Execution;

public class AppExecutionContext
{
    public required AppDbContext Db { get; init; }
    public required AppOptions AppOptions { get; init; }
    public required IUserContextService UserContext { get; init; }
    public required MessageLocalizer Localizer { get; init; }
    public required ILogDispatcher LogDispatcher { get; init; }
    public required ISafeExecutor SafeExecutor { get; init; }
    public required IGuardChecker GuardChecker { get; init; }

    // Optional dependencies
    public IAuditLogger? AuditLogger { get; init; }
    public IRedisLimiterService? RedisLimiterService { get; init; }
    public IRedisLockService? RedisLockService { get; init; }
    public EmailContext? EmailContext { get; init; }
    public SqsEmailProducer? EmailQueueProducer { get; init; }
}
using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Application.Interfaces.Services;
using SmartRetail360.Infrastructure.AuditLogging;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Infrastructure.Services.Messaging;
using SmartRetail360.Infrastructure.Services.Notifications.Configuration;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Options;

namespace SmartRetail360.Infrastructure.Services.AccountRegistration.Models;

public class TenantRegistrationDependencies
{
    public required AppDbContext Db { get; init; }
    public required IUserContextService UserContext { get; init; }
    public required MessageLocalizer Localizer { get; init; }
    public required EmailContext EmailContext { get; init; }
    public required IRedisLockService RedisLockService { get; init; }
    public required AppOptions AppOptions { get; init; }
    public IAuditLogger AuditLogger { get; init; } 
    public required ILogDispatcher LogDispatcher { get; init; }
    public required SqsEmailProducer EmailQueueProducer { get; init; }
    public required ISafeExecutor SafeExecutor { get; init; }
}
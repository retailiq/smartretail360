namespace SmartRetail360.Infrastructure.Common.DependencyInjection;

using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.Common.UserContext;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Application.Interfaces.Redis;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Options;

public class BaseDependencies
{
    public IPlatformContextService PlatformContext { get; set; }
    public MessageLocalizer Localizer { get; set; }
    public ISafeExecutor SafeExecutor { get; set; }
    public IGuardChecker GuardChecker { get; set; }
    public IRedisOperationService RedisOperation { get; set; }
    public AppDbContext Db { get; set; }
    public IUserContextService UserContext { get; set; }
    public AppOptions AppOptions { get; set; }
    public ILogDispatcher LogDispatcher { get; set; }
}
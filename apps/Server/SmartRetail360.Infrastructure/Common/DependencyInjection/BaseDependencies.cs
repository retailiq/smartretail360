using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Caching.Interfaces;
using SmartRetail360.Execution;
using SmartRetail360.Logging.Abstractions;
using SmartRetail360.Logging.Interfaces;
using SmartRetail360.Persistence;
using SmartRetail360.Persistence.Data;
using SmartRetail360.Platform.Interfaces;
using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Options;

namespace SmartRetail360.Infrastructure.Common.DependencyInjection;
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
    public IRefreshTokenService RefreshTokenService { get; set; }
}
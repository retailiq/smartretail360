using Microsoft.Extensions.DependencyInjection;
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

public static class DependencyBuilder
{
    public static T Build<T>(IServiceProvider sp, Action<T> configureExtra) where T : BaseDependencies, new()
    {
        var instance = new T
        {
            Db = sp.GetRequiredService<AppDbContext>(),
            UserContext = sp.GetRequiredService<IUserContextService>(),
            Localizer = sp.GetRequiredService<MessageLocalizer>(),
            AppOptions = sp.GetRequiredService<AppOptions>(),
            LogDispatcher = sp.GetRequiredService<ILogDispatcher>(),
            SafeExecutor = sp.GetRequiredService<ISafeExecutor>(),
            GuardChecker = sp.GetRequiredService<IGuardChecker>(),
            RedisOperation = sp.GetRequiredService<IRedisOperationService>(),
            PlatformContext = sp.GetRequiredService<IPlatformContextService>(),
            RefreshTokenService = sp.GetRequiredService<IRefreshTokenService>()
        };

        configureExtra(instance);

        return instance;
    }
}
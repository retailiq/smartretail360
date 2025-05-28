using Microsoft.Extensions.DependencyInjection;
using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.Common.UserContext;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Application.Interfaces.Redis;
using SmartRetail360.Infrastructure.Data;
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
            PlatformContext = sp.GetRequiredService<IPlatformContextService>()
        };

        configureExtra(instance);

        return instance;
    }
}
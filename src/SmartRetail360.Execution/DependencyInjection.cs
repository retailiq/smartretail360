using Microsoft.Extensions.DependencyInjection;

namespace SmartRetail360.Execution;

public static class DependencyInjection
{
    public static IServiceCollection AddExecution(this IServiceCollection services)
    {
        services.AddScoped<ISafeExecutor, SafeExecutor>();
        services.AddScoped<IGuardChecker, GuardChecker>();
        
        return services;
    }
}
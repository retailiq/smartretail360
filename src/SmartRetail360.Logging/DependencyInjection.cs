using Microsoft.Extensions.DependencyInjection;
using SmartRetail360.Logging.Abstractions;
using SmartRetail360.Logging.Interfaces;
using SmartRetail360.Logging.Services;
using SmartRetail360.Logging.Services.Context;
using SmartRetail360.Logging.Services.Dispatcher;
using SmartRetail360.Logging.Services.Loggers;
using SmartRetail360.Logging.Services.Policies;

namespace SmartRetail360.Logging;

public static class DependencyInjection
{
    public static IServiceCollection AddLogging(this IServiceCollection services)
    {
        services.AddScoped<ILogContextAccessor, LogContextAccessor>();
        services.AddScoped<ILogDispatcher, LogDispatcher>();
        services.AddScoped<IAuditLogger, AuditLogger>();
        services.AddSingleton<ILogWritePolicyProvider, DefaultLogWritePolicyProvider>();
        services.AddScoped<ILogWriter, DefaultLogWriter>();
        
        return services;
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartRetail360.Infrastructure.Common.DependencyInjection;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Options;

namespace SmartRetail360.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddShared(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped(typeof(Lazy<>), typeof(LazyResolver<>));
        
        services.AddScoped<MessageLocalizer>();
        
        services.Configure<AppOptions>(config.GetSection(GeneralConstants.App));
        
        return services;
    }
}
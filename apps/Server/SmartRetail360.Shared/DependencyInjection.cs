using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Options;

namespace SmartRetail360.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedLayer(this IServiceCollection services, IConfiguration config)
    {
        // 注册多语言支持
        services.AddSingleton<MessageLocalizer>();
        
        services.Configure<AppOptions>(config.GetSection("App"));
        
        return services;
    }
}
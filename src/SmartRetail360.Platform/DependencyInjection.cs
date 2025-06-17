using Microsoft.Extensions.DependencyInjection;
using SmartRetail360.Platform.Interfaces;
using SmartRetail360.Platform.Services;

namespace SmartRetail360.Platform;

public static class DependencyInjection
{
    public static IServiceCollection AddPlatform(this IServiceCollection services)
    {
        services.AddScoped<IPlatformContextService, PlatformContextService>();
        services.AddScoped<IAccountSupportService, AccountSupportService>();
        return services;
    }
}
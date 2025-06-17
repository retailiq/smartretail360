using Microsoft.Extensions.DependencyInjection;
using SmartRetail360.Shared.Contexts.User;

namespace SmartRetail360.Shared.Contexts;


public static class DependencyInjection
{
    public static IServiceCollection AddSharedContexts(this IServiceCollection services)
    {
        services.AddScoped<IUserContextService, UserContextService>();
        return services;
    }
}
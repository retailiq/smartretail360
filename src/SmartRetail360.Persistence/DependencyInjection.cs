using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartRetail360.Persistence.Data;
using SmartRetail360.Persistence.Interceptors;

namespace SmartRetail360.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<EntityTimestampsInterceptor>();

        services.AddDbContext<AppDbContext>((provider, options) =>
        {
            var interceptor = provider.GetRequiredService<EntityTimestampsInterceptor>();
            options.UseNpgsql(config.GetConnectionString("DefaultConnection"))
                .AddInterceptors(interceptor);
        });
        return services;
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartRetail360.Caching.Abstractions;
using SmartRetail360.Caching.Interfaces;
using SmartRetail360.Caching.Interfaces.Caching;
using SmartRetail360.Caching.Services;
using StackExchange.Redis;

namespace SmartRetail360.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration config)
    {
        var redis = ConnectionMultiplexer.Connect(config["Redis:ConnectionString"]!);
        services.AddSingleton<IConnectionMultiplexer>(redis);
        
        services.AddScoped<IRedisLimiterService, RedisRedisLimiterService>();
        services.AddScoped<IRedisLockService, RedisRedisLockService>();
        services.AddScoped<IRedisLogSamplingService, RedisLogSamplingService>();
        services.AddScoped<IRoleCacheService, RoleCacheService>();
        services.AddScoped<IAccountTokenCacheService, AccountTokenCacheService>();
        services.AddScoped<IRedisOperationService, RedisOperationService>();
        services.AddScoped<ILoginFailureLimiter, LoginFailureLimiter>();

        return services;
    }
}
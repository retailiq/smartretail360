using SmartRetail360.Application.Interfaces.Services;
using StackExchange.Redis;

namespace SmartRetail360.Infrastructure.Services.Common;

public class RedisLimiterService : ILimiterService
{
    private readonly IDatabase _redis;

    public RedisLimiterService(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
    }

    public async Task<bool> IsLimitedAsync(string key)
    {
        return await _redis.KeyExistsAsync(key);
    }

    public async Task SetLimitAsync(string key, TimeSpan ttl)
    {
        await _redis.StringSetAsync(key, "", ttl);
    }
}
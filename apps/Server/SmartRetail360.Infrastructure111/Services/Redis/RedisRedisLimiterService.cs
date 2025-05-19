using SmartRetail360.Application.Interfaces.Services;
using StackExchange.Redis;

namespace SmartRetail360.Infrastructure.Services.Redis;

public class RedisRedisLimiterService : IRedisLimiterService
{
    private readonly IDatabase _redis;

    public RedisRedisLimiterService(IConnectionMultiplexer redis)
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
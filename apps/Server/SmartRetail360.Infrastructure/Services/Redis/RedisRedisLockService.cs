using SmartRetail360.Application.Interfaces.Redis;
using StackExchange.Redis;

namespace SmartRetail360.Infrastructure.Services.Redis;

public class RedisRedisLockService : IRedisLockService
{
    private readonly IDatabase _redis;

    public RedisRedisLockService(IConnectionMultiplexer connection)
    {
        _redis = connection.GetDatabase();
    }

    public async Task<bool> AcquireLockAsync(string key, TimeSpan ttl)
    {
        return await _redis.StringSetAsync(key, "1", ttl, When.NotExists);
    }

    public async Task ReleaseLockAsync(string key)
    {
        await _redis.KeyDeleteAsync(key);
    }
}
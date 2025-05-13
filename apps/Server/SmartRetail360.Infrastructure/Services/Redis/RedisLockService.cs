using SmartRetail360.Application.Interfaces.Services;
using StackExchange.Redis;

namespace SmartRetail360.Infrastructure.Services.Common;

public class RedisLockService : ILockService
{
    private readonly IDatabase _redis;

    public RedisLockService(IConnectionMultiplexer connection)
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
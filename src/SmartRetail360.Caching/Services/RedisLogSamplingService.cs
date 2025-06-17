using SmartRetail360.Caching.Abstractions;
using StackExchange.Redis;

namespace SmartRetail360.Caching.Services;

public class RedisLogSamplingService : IRedisLogSamplingService
{
    private readonly IDatabase _db;

    public RedisLogSamplingService(IConnectionMultiplexer connection)
    {
        _db = connection.GetDatabase();
    }

    public Task<bool> ExistsAsync(string key)
    {
        return _db.KeyExistsAsync(key);
    }

    public Task SetStringAsync(string key, string value, TimeSpan? expiry = null)
    {
        return _db.StringSetAsync(key, value, expiry);
    }
}
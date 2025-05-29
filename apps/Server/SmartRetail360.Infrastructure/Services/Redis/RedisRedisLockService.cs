using Microsoft.Extensions.Options;
using SmartRetail360.Application.Interfaces.Redis;
using SmartRetail360.Shared.Options;
using SmartRetail360.Shared.Redis;
using StackExchange.Redis;

namespace SmartRetail360.Infrastructure.Services.Redis;

public class RedisRedisLockService : IRedisLockService
{
    private readonly IDatabase _redis;
    private readonly AppOptions _options;

    public RedisRedisLockService(IConnectionMultiplexer connection, IOptions<AppOptions> options)
    {
        _redis = connection.GetDatabase();
        _options = options.Value;
    }

    public async Task<bool> AcquireUserLoginLockAsync(string email)
    {
        var key = RedisKeys.UserLoginLock(email);
        var ttl = TimeSpan.FromSeconds(_options.UserLoginLockTtlSeconds);
        return await _redis.StringSetAsync(key, "1", ttl, When.NotExists);
    }

    public async Task ReleaseUserLoginLockAsync(string email)
    {
        var key = RedisKeys.UserLoginLock(email);
        await _redis.KeyDeleteAsync(key);
    }

    public async Task<bool> AcquireRegistrationLockAsync(string email)
    {
        var key = RedisKeys.RegisterAccountLock(email);
        var ttl = TimeSpan.FromSeconds(_options.RegistrationLockTtlSeconds);
        return await _redis.StringSetAsync(key, "1", ttl, When.NotExists);
    }

    public async Task ReleaseRegistrationLockAsync(string email)
    {
        var key = RedisKeys.RegisterAccountLock(email);
        await _redis.KeyDeleteAsync(key);
    }
}
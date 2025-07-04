using Microsoft.Extensions.Options;
using SmartRetail360.Caching.Interfaces;
using SmartRetail360.Shared.Options;
using SmartRetail360.Shared.Redis;
using StackExchange.Redis;

namespace SmartRetail360.Caching.Services;

public class RedisRedisLimiterService : IRedisLimiterService
{
    private readonly IDatabase _redis;
    private readonly AppOptions _options;

    public RedisRedisLimiterService(
        IConnectionMultiplexer redis,
        IOptions<AppOptions> options)
    {
        _redis = redis.GetDatabase();
        _options = options.Value;
    }

    public async Task<bool> IsEmailResendLimitedAsync(string email)
    {
        var key = RedisKeys.ResendAccountActivationEmail(email);
        return await _redis.KeyExistsAsync(key);
    }

    public async Task SetEmailResendLimitAsync(string email)
    {
        var key = RedisKeys.ResendAccountActivationEmail(email);
        var ttl = TimeSpan.FromMinutes(_options.EmailSendLimitMinutes);
        await _redis.StringSetAsync(key, "", ttl);
    }

    public async Task<bool> IsAccountActivationLimitedAsync(string token)
    {
        var key = RedisKeys.VerifyEmailRateLimit(token);
        return await _redis.KeyExistsAsync(key);
    }

    public async Task SetAccountActivationLimitAsync(string token)
    {
        var key = RedisKeys.VerifyEmailRateLimit(token);
        var ttl = TimeSpan.FromMinutes(_options.EmailValidityPeriodMinutes);
        await _redis.StringSetAsync(key, "", ttl);
    }
}
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using SmartRetail360.Application.Interfaces.Redis;
using SmartRetail360.Shared.Options;
using SmartRetail360.Shared.Redis;

namespace SmartRetail360.Infrastructure.Services.Redis;

public class LoginFailureLimiter : ILoginFailureLimiter
{
    private readonly IDatabase _redis;
    private readonly AppOptions _appOptions;

    public LoginFailureLimiter(
        IConnectionMultiplexer connection,
        IOptions<AppOptions> appOptions)
    {
        _redis = connection.GetDatabase();
        _appOptions = appOptions.Value;
    }

    public async Task<bool> IsLockedAsync(string email)
    {
        var lockKey = RedisKeys.UserLoginLock(email.ToLower());
        return await _redis.KeyExistsAsync(lockKey);
    }

    public async Task<int> IncrementFailureAsync(string email)
    {
        email = email.ToLower();
        var failKey = RedisKeys.UserLoginFailures(email);
        var lockKey = RedisKeys.UserLoginLock(email);

        var count = (int)await _redis.StringIncrementAsync(failKey);
        var threshold = _appOptions.UserLoginFailureThreshold;

        if (count >= threshold)
        {
            await _redis.StringSetAsync(lockKey, "1"); // Forever lock
        }

        return count;
    }

    public async Task ResetFailuresAsync(string email)
    {
        email = email.ToLower();
        var failKey = RedisKeys.UserLoginFailures(email);
        var lockKey = RedisKeys.UserLoginLock(email);
        await _redis.KeyDeleteAsync(failKey);
        await _redis.KeyDeleteAsync(lockKey);
    }
}
using SmartRetail360.Application.Interfaces.Redis;
using SmartRetail360.Application.Interfaces.Caching;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Infrastructure.Services.Redis;

public class RedisOperationService : IRedisOperationService
{
    private readonly IRedisLimiterService _limiter;
    private readonly IRedisLockService _locker;
    private readonly IRedisLogSamplingService _logSampler;
    private readonly IRoleCacheService _roleCache;
    private readonly IActivationTokenCacheService _tokenCache;

    public RedisOperationService(
        IRedisLimiterService limiter,
        IRedisLockService locker,
        IRedisLogSamplingService logSampler,
        IRoleCacheService roleCache,
        IActivationTokenCacheService tokenCache)
    {
        _limiter = limiter;
        _locker = locker;
        _logSampler = logSampler;
        _roleCache = roleCache;
        _tokenCache = tokenCache;
    }

    // Limit methods
    public Task<bool> IsLimitedAsync(string key) => _limiter.IsLimitedAsync(key);
    public Task SetLimitAsync(string key, TimeSpan window) => _limiter.SetLimitAsync(key, window);

    // Lock methods
    public Task<bool> AcquireLockAsync(string key, TimeSpan expiry) => _locker.AcquireLockAsync(key, expiry);
    public Task ReleaseLockAsync(string key) => _locker.ReleaseLockAsync(key);

    // Log sampling
    public Task<bool> LogSampleExistsAsync(string key) => _logSampler.ExistsAsync(key);
    public Task SetLogSampleAsync(string key, string value, TimeSpan? expiry = null) => _logSampler.SetStringAsync(key, value, expiry);

    // Role cache
    public Task<Role?> GetSystemRoleAsync(SystemRoleType roleType) => _roleCache.GetSystemRoleAsync(roleType);

    // Activation token cache
    public Task SetActivationTokenAsync(AccountActivationToken tokenEntity, TimeSpan ttl) => _tokenCache.SetTokenAsync(tokenEntity, ttl);
    public Task<AccountActivationToken?> GetActivationTokenAsync(string token) => _tokenCache.GetTokenAsync(token);
    public Task InvalidateActivationTokenAsync(string token) => _tokenCache.InvalidateTokenAsync(token);
}
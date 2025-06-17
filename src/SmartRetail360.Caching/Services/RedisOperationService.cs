using SmartRetail360.Caching.Interfaces;
using SmartRetail360.Caching.Interfaces.Caching;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Caching.Services;

public class RedisOperationService : IRedisOperationService
{
    private readonly IRedisLimiterService _limiter;
    private readonly IRedisLockService _locker;
    private readonly IRoleCacheService _roleCache;
    private readonly IActivationTokenCacheService _tokenCache;
    private readonly ILoginFailureLimiter _loginFailureLimiter;

    public RedisOperationService(
        IRedisLimiterService limiter,
        IRedisLockService locker,
        IRoleCacheService roleCache,
        IActivationTokenCacheService tokenCache,
        ILoginFailureLimiter loginFailureLimiter)
    {
        _limiter = limiter;
        _locker = locker;
        _roleCache = roleCache;
        _tokenCache = tokenCache;
        _loginFailureLimiter = loginFailureLimiter;
    }
    
    // Email resend rate limit
    public Task<bool> IsEmailResendLimitedAsync(string email) => _limiter.IsEmailResendLimitedAsync(email);
    public Task SetEmailResendLimitAsync(string email) => _limiter.SetEmailResendLimitAsync(email);

    // Activation verify rate limit
    public Task<bool> IsAccountActivationLimitedAsync(string token) => _limiter.IsAccountActivationLimitedAsync(token);
    public Task SetAccountActivationLimitAsync(string token) => _limiter.SetAccountActivationLimitAsync(token);

    // Lock methods
    public Task<bool> AcquireUserLoginLockAsync(string email) => _locker.AcquireUserLoginLockAsync(email);
    public Task ReleaseUserLoginLockAsync(string email) => _locker.ReleaseUserLoginLockAsync(email);

    public Task<bool> AcquireRegistrationLockAsync(string email) => _locker.AcquireRegistrationLockAsync(email);
    public Task ReleaseRegistrationLockAsync(string email) => _locker.ReleaseRegistrationLockAsync(email);

    // Role cache
    public Task<Role?> GetSystemRoleAsync(SystemRoleType roleType) => _roleCache.GetSystemRoleAsync(roleType);

    public Task<List<Role>> GetSystemRolesByIdsAsync(List<Guid> roleIds) =>
        _roleCache.GetSystemRolesByIdsAsync(roleIds);

    // Activation token cache
    public Task SetActivationTokenAsync(AccountActivationToken tokenEntity) => _tokenCache.SetTokenAsync(tokenEntity);
    public Task<AccountActivationToken?> GetActivationTokenAsync(string token) => _tokenCache.GetTokenAsync(token);
    public Task InvalidateActivationTokenAsync(string token) => _tokenCache.InvalidateTokenAsync(token);

    // User Login Lock
    public Task<bool> IsUserLoginLockedAsync(string email) =>
        _loginFailureLimiter.IsLockedAsync(email);

    public Task<int> IncrementUserLoginFailureAsync(string email) =>
        _loginFailureLimiter.IncrementFailureAsync(email);

    public Task ResetUserLoginFailuresAsync(string email) =>
        _loginFailureLimiter.ResetFailuresAsync(email);
}
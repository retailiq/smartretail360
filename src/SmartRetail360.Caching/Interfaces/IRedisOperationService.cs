using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Caching.Interfaces;

public interface IRedisOperationService
{
    // Email resend rate limit
    Task<bool> IsEmailResendLimitedAsync(string email);
    Task SetEmailResendLimitAsync(string email);

    // Activation token verification rate limit
    Task<bool> IsAccountActivationLimitedAsync(string token);
    Task SetAccountActivationLimitAsync(string token);

    // Distributed lock
    Task<bool> AcquireUserLoginLockAsync(string email);
    Task ReleaseUserLoginLockAsync(string email);

    // Registration lock
    Task<bool> AcquireRegistrationLockAsync(string email);
    Task ReleaseRegistrationLockAsync(string email);

    // Role cache
    Task<Role?> GetSystemRoleAsync(SystemRoleType roleType);
    Task<List<Role>> GetSystemRolesByIdsAsync(List<Guid> roleIds);
    
    // Token cache
    Task SetActivationTokenAsync(AccountToken tokenEntity);
    Task<AccountToken?> GetActivationTokenAsync(string token);
    Task InvalidateActivationTokenAsync(string token);
    
    // User Login Lock
    Task<bool> IsUserLoginLockedAsync(string email);
    Task<int> IncrementUserLoginFailureAsync(string email);
    Task ResetUserLoginFailuresAsync(string email);
}
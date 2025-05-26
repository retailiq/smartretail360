using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Interfaces.Redis;

public interface IRedisOperationService
{
    // Rate limit
    Task<bool> IsLimitedAsync(string key);
    Task SetLimitAsync(string key, TimeSpan window);

    // Distributed lock
    Task<bool> AcquireLockAsync(string key, TimeSpan expiry);
    Task ReleaseLockAsync(string key);

    // Log sampling
    Task<bool> LogSampleExistsAsync(string key);
    Task SetLogSampleAsync(string key, string value, TimeSpan? expiry = null);

    // Role cache
    Task<Role?> GetSystemRoleAsync(SystemRoleType roleType);
    Task<List<Role>> GetSystemRolesByIdsAsync(List<Guid> roleIds);
    
    // Token cache
    Task SetActivationTokenAsync(AccountActivationToken tokenEntity, TimeSpan ttl);
    Task<AccountActivationToken?> GetActivationTokenAsync(string token);
    Task InvalidateActivationTokenAsync(string token);
}
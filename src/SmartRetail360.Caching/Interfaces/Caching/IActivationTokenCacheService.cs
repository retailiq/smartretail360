using SmartRetail360.Domain.Entities;

namespace SmartRetail360.Caching.Interfaces.Caching;

public interface IActivationTokenCacheService
{
    Task SetTokenAsync(AccountActivationToken tokenEntity);
    Task<AccountActivationToken?> GetTokenAsync(string token);
    Task InvalidateTokenAsync(string token);
}
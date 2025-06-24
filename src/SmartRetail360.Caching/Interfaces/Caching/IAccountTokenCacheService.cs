using SmartRetail360.Domain.Entities;

namespace SmartRetail360.Caching.Interfaces.Caching;

public interface IAccountTokenCacheService
{
    Task SetTokenAsync(AccountToken tokenEntity);
    Task<AccountToken?> GetTokenAsync(string token);
    Task InvalidateTokenAsync(string token);
}
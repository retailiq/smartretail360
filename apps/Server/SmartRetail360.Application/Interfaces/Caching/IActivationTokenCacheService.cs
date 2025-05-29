namespace SmartRetail360.Application.Interfaces.Caching;

using SmartRetail360.Domain.Entities;

public interface IActivationTokenCacheService
{
    Task SetTokenAsync(AccountActivationToken tokenEntity);
    Task<AccountActivationToken?> GetTokenAsync(string token);
    Task InvalidateTokenAsync(string token);
}
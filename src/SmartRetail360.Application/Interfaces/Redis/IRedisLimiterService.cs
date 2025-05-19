namespace SmartRetail360.Application.Interfaces.Services;


public interface IRedisLimiterService
{
    Task<bool> IsLimitedAsync(string key);
    Task SetLimitAsync(string key, TimeSpan ttl);
}
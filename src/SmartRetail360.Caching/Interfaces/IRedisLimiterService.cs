namespace SmartRetail360.Caching.Interfaces;

public interface IRedisLimiterService
{
    Task<bool> IsEmailResendLimitedAsync(string email);
    Task SetEmailResendLimitAsync(string email);

    Task<bool> IsAccountActivationLimitedAsync(string token);
    Task SetAccountActivationLimitAsync(string token);
}
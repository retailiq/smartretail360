namespace SmartRetail360.Application.Interfaces.Redis;

public interface IRedisLimiterService
{
    Task<bool> IsEmailResendLimitedAsync(string email);
    Task SetEmailResendLimitAsync(string email);

    Task<bool> IsAccountActivationLimitedAsync(string token);
    Task SetAccountActivationLimitAsync(string token);
}
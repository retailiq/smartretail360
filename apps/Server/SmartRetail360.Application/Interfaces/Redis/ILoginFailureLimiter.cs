namespace SmartRetail360.Application.Interfaces.Redis;

public interface ILoginFailureLimiter
{
    Task<bool> IsLockedAsync(string lockKey);
    Task<int> IncrementFailureAsync(string failKey, string lockKey);
    Task ResetFailuresAsync(string failKey, string lockKey);
}
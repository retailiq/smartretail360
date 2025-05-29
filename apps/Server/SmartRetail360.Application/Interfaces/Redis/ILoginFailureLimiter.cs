namespace SmartRetail360.Application.Interfaces.Redis;

public interface ILoginFailureLimiter
{
    Task<bool> IsLockedAsync(string email);
    Task<int> IncrementFailureAsync(string email);
    Task ResetFailuresAsync(string email);
}
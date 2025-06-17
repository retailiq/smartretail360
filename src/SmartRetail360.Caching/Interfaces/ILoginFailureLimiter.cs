namespace SmartRetail360.Caching.Interfaces;

public interface ILoginFailureLimiter
{
    Task<bool> IsLockedAsync(string email);
    Task<int> IncrementFailureAsync(string email);
    Task ResetFailuresAsync(string email);
}
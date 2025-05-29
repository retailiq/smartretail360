namespace SmartRetail360.Application.Interfaces.Redis;

public interface IRedisLockService
{
    Task<bool> AcquireUserLoginLockAsync(string email);
    Task ReleaseUserLoginLockAsync(string email);

    Task<bool> AcquireRegistrationLockAsync(string email);
    Task ReleaseRegistrationLockAsync(string email);
}
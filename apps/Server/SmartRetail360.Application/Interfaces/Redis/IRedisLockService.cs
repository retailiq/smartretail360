namespace SmartRetail360.Application.Interfaces.Redis;

public interface IRedisLockService
{
    Task<bool> AcquireLockAsync(string key, TimeSpan ttl);
    Task ReleaseLockAsync(string key);
}
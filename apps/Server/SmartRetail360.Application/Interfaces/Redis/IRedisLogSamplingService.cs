namespace SmartRetail360.Application.Interfaces.Redis;

public interface IRedisLogSamplingService
{
    Task<bool> ExistsAsync(string key);
    Task SetStringAsync(string key, string value, TimeSpan? expiry = null);
}
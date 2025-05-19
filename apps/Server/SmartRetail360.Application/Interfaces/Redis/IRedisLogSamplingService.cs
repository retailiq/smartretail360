namespace SmartRetail360.Application.Interfaces.Common;

public interface IRedisLogSamplingService
{
    Task<bool> ExistsAsync(string key);
    Task SetStringAsync(string key, string value, TimeSpan? expiry = null);
}
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Options;

namespace SmartRetail360.Caching.Abstractions;

public interface IRedisLogSamplingService
{
    Task<bool> ExistsAsync(string key);
    Task SetStringAsync(string key, string value, TimeSpan? expiry = null);
}
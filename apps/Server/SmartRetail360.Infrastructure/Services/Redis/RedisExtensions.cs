using SmartRetail360.Application.Interfaces.Common;

namespace SmartRetail360.Infrastructure.Services.Redis;

public static class RedisExtensions
{
    public static async Task<bool> ShouldSampleAsync(this IRedisLogSamplingService redisLogSampling, string key, TimeSpan interval)
    {
        if (await redisLogSampling.ExistsAsync(key)) return false;
        await redisLogSampling.SetStringAsync(key, "1", interval);
        return true;
    }
}
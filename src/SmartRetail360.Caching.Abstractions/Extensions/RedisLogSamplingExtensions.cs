using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Options;
using SmartRetail360.Shared.Redis;

namespace SmartRetail360.Caching.Abstractions.Extensions;

public static class RedisLogSamplingExtensions
{
    public static async Task<bool> ShouldSampleAsync(
        this IRedisLogSamplingService redisLogSampling,
        LogEventType eventType,
        string? reason,
        AppOptions options)
    {
        var samplingKey = RedisKeys.LogSampling(eventType, reason ?? GeneralConstants.Unknown);
        var interval = TimeSpan.FromMinutes(options.LogSamplingLimitMinutes);

        if (await redisLogSampling.ExistsAsync(samplingKey)) return false;

        await redisLogSampling.SetStringAsync(samplingKey, "1", interval);
        return true;
    }
}
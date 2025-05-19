using Microsoft.Extensions.Options;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Infrastructure.Services.Redis;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Logging;
using SmartRetail360.Shared.Options;
using SmartRetail360.Shared.Redis;

namespace SmartRetail360.Infrastructure.Logging.Dispatcher;

public class LogDispatcher : ILogDispatcher
{
    private readonly Dictionary<LogEventType, ILogEventHandler> _handlers;
    private readonly IRedisLogSamplingService _redisLogSampling;
    private readonly AppOptions _appOptions;

    public LogDispatcher(IEnumerable<
            ILogEventHandler> handlers,
        IRedisLogSamplingService redisLogSampling,
        IOptions<AppOptions> appOptions)
    {
        _handlers = handlers.ToDictionary(h => h.EventType);
        _redisLogSampling = redisLogSampling;
        _appOptions = appOptions.Value;
    }

    public async Task Dispatch(LogEventType eventType, string? reason = null)
    {
        if (_handlers.TryGetValue(eventType, out var handler))
        {
            var context = new LogContext
            {
                LogId = Guid.NewGuid().ToString(),
                Reason = reason,
            };

            var samplingKey = RedisKeys.LogSampling(eventType, reason ?? "unknown");

            if (_appOptions.LogSamplingLimitMinutes <= 0 ||
                await _redisLogSampling.ShouldSampleAsync(samplingKey,
                    TimeSpan.FromMinutes(_appOptions.LogSamplingLimitMinutes)))
            {
                await handler.HandleAsync(context);
            }
        }
    }
}
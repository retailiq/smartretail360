using Microsoft.Extensions.Options;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Application.Interfaces.Redis;
using SmartRetail360.Infrastructure.Services.Redis;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Logging;
using SmartRetail360.Shared.Options;

namespace SmartRetail360.Infrastructure.Logging.Dispatcher;

public class LogDispatcher : ILogDispatcher
{
    private readonly IRedisLogSamplingService _redisLogSampling;
    private readonly AppOptions _appOptions;
    private readonly ILogWritePolicyProvider _policyProvider;
    private readonly ILogWriter _writer;

    public LogDispatcher(
        IRedisLogSamplingService redisLogSampling,
        IOptions<AppOptions> appOptions,
        ILogWritePolicyProvider policyProvider,
        ILogWriter writer)
    {
        _redisLogSampling = redisLogSampling;
        _appOptions = appOptions.Value;
        _policyProvider = policyProvider;
        _writer = writer;
    }

    public async Task Dispatch(LogEventType eventType, string? reason = null)
    {
        if (_appOptions.LogSamplingLimitMinutes <= 0 ||
            await _redisLogSampling.ShouldSampleAsync(eventType, reason, _appOptions))
        {
            var policy = _policyProvider.GetPolicy(eventType, reason);
        
            var context = new LogContext
            {
                Reason = reason
            };
        
            await _writer.WriteAsync(context, policy);
        }
    }
}
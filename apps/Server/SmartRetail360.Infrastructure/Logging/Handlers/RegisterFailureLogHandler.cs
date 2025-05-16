using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Infrastructure.Logging.Policies;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Logging;

namespace SmartRetail360.Infrastructure.Logging.Handlers;

public class RegisterFailureLogHandler : ILogEventHandler
{
    private readonly ILogWritePolicyProvider _policyProvider;
    private readonly ILogWriter _logWriter;
    
    public LogEventType EventType => LogEventType.RegisterFailure;

    public RegisterFailureLogHandler(
        ILogWritePolicyProvider policyProvider,
        ILogWriter logWriter)
    {
        _policyProvider = policyProvider;
        _logWriter = logWriter;
    }

    public Task HandleAsync(LogContext context)
    {
        var policy = _policyProvider.GetPolicy(EventType, context.Reason);
        context.IsSuccess = false;
        return _logWriter.WriteAsync(context, policy);
    }
}
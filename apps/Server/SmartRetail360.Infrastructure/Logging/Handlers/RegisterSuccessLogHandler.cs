using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Infrastructure.Logging.Policies;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Logging;

namespace SmartRetail360.Infrastructure.Logging.Handlers;

public class RegisterSuccessLogHandler : ILogEventHandler
{
    public LogEventType EventType => LogEventType.RegisterSuccess;

    private readonly ILogWritePolicyProvider _policyProvider;
    private readonly ILogWriter _logWriter;
    private readonly IUserContextService _userContext;

    public RegisterSuccessLogHandler(
        ILogWritePolicyProvider policyProvider,
        ILogWriter logWriter,
        IUserContextService userContext)
    {
        _policyProvider = policyProvider;
        _logWriter = logWriter;
        _userContext = userContext;
    }

    public Task HandleAsync(LogContext context)
    {
        var policy = _policyProvider.GetPolicy(EventType, null);

        context.IsSuccess = true;
        context.Action = _userContext.AccountType == AccountType.UserAccount
            ? "USER_REGISTER"
            : "REGISTER_TENANT";

        return _logWriter.WriteAsync(context, policy);
    }
}
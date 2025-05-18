using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Infrastructure.Logging.Policies;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Logging;

namespace SmartRetail360.Infrastructure.Logging.Handlers;

public class AccountActivateFailureLogHandler : ILogEventHandler
{
    public LogEventType EventType => LogEventType.AccountActivateFailure;

    private readonly ILogWritePolicyProvider _policyProvider;
    private readonly ILogWriter _logWriter;
    private readonly IUserContextService _userContext;

    public AccountActivateFailureLogHandler(
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
        var policy = _policyProvider.GetPolicy(EventType, context.Reason);
        context.Action = _userContext.AccountType == AccountType.UserAccount
            ? LogActions.UserAccountActivate
            : LogActions.TenantAccountActivate;
        return _logWriter.WriteAsync(context, policy);
    }
}
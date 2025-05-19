using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Infrastructure.Logging;

public class DefaultLogActionResolver : ILogActionResolver
{
    public string ResolveAction(LogEventType eventType, AccountType? accountType)
    {
        return (eventType, accountType) switch
        {
            (LogEventType.RegisterSuccess, AccountType.UserAccount) => LogActions.UserRegister,
            (LogEventType.RegisterSuccess, AccountType.TenantAccount) => LogActions.TenantRegister,
            (LogEventType.RegisterFailure, AccountType.UserAccount) => LogActions.UserRegister,
            (LogEventType.RegisterFailure, AccountType.TenantAccount) => LogActions.TenantRegister,
            _ => "UNKNOWN_ACTION"
        };
    }
}
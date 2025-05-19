using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Interfaces.Common;

public interface ILogActionResolver
{
    string ResolveAction(LogEventType eventType, AccountType? accountType);
}
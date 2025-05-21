using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Interfaces.Logging;

public interface ILogActionResolver
{
    string ResolveAction(LogEventType eventType, AccountType? accountType);
}
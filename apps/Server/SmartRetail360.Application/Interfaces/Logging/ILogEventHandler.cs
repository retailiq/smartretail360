using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Logging;

namespace SmartRetail360.Application.Interfaces.Logging;

public interface ILogEventHandler
{
    LogEventType EventType { get; }
    Task HandleAsync(LogContext context);
}
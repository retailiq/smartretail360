using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Logging.Abstractions;

public interface ILogDispatcher
{
    Task Dispatch(LogEventType eventType, string? reason = null);
}
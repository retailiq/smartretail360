using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Interfaces.Logging;

public interface ILogDispatcher
{
    Task Dispatch(LogEventType eventType, string? reason = null);
}
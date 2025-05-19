using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Interfaces.Common;

public interface ILogDispatcher
{
    Task Dispatch(LogEventType eventType, string? reason = null);
}
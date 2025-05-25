using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Logging;

namespace SmartRetail360.Application.Interfaces.Logging;

public interface ILogWritePolicyProvider
{
    LogWriteRule GetPolicy(LogEventType eventType, string? reason = null);
}
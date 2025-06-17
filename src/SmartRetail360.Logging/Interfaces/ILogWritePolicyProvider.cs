using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Logging;

namespace SmartRetail360.Logging.Interfaces;

public interface ILogWritePolicyProvider
{
    LogWriteRule GetPolicy(LogEventType eventType, string? reason = null);
}
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Logging;

namespace SmartRetail360.Infrastructure.Logging.Policies;

public interface ILogWritePolicyProvider
{
    LogWriteRule GetPolicy(LogEventType eventType, string? reason = null);
}
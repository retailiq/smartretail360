using SmartRetail360.Shared.Logging;

namespace SmartRetail360.Logging.Interfaces;

public interface IAuditLogger
{
    Task LogAsync(AuditContext ctx);
}
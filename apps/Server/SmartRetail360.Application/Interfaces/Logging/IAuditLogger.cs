using SmartRetail360.Shared.Logging;

namespace SmartRetail360.Application.Interfaces.Logging;

public interface IAuditLogger
{
    Task LogAsync(AuditContext ctx);
}
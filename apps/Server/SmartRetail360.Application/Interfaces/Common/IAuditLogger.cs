using SmartRetail360.Domain.Entities;

namespace SmartRetail360.Application.Interfaces.Common;

public interface IAuditLogger
{
    Task LogAsync(AuditLog entry);
}
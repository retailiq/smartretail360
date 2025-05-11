namespace SmartRetail360.Application.Interfaces.Common;

public interface IUserContextService
{
    Guid? UserId { get; }
    Guid? TenantId { get; }
    Guid? RoleId { get; }
    string? TraceId { get; }
    string? Locale { get; }
    
    void LogAllContext();
}
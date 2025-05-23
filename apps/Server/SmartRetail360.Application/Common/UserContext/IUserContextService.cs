using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Common.UserContext;

public interface IUserContextService
{
    Guid? UserId { get; set; }
    Guid? TenantId { get; set; }
    Guid? RoleId { get; set; }
    string? TraceId { get; set; }
    string? Locale { get; set; }
    string IpAddress { get; set; }
    string? Module { get; set; }
    string? Email { get; set; }
    string? ErrorStack { get; set; } 
    string? Action { get; set; }
    string? RoleName { get; set; }
    string? LogId { get; set; }
    string? UserName { get; set; }
    
    void Inject(
        Guid? userId = null,
        Guid? tenantId = null,
        Guid? roleId = null,
        string? traceId = null,
        string? locale = null,
        string? module = null,
        string? email = null,
        string? errorStack = null,
        string? ipAddress = null,
        string? action = null,
        string? roleName = null,
        string? logId = null,
        string? userName = null
    );
}
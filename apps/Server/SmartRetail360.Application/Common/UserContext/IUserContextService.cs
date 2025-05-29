using SmartRetail360.Shared.Context;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Common.UserContext;

public interface IUserContextService
{
    Guid? UserId { get; set; }
    Guid? TenantId { get; set; }
    Guid? RoleId { get; set; }
    string TraceId { get; set; }
    string? Locale { get; set; }
    string IpAddress { get; set; }
    string? Module { get; set; }
    string? Email { get; set; }
    string? ErrorStack { get; set; } 
    string? Action { get; set; }
    string? RoleName { get; set; }
    string? LogId { get; set; }
    string? UserName { get; set; }
    LogEventType? LogEventType { get; set; }
    
    void Inject(UserExecutionContext context);
}
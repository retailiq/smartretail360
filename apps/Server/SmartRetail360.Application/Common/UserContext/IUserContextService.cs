using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Enums.AccessControl;

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
    DefaultEnvironmentType Env { get; set; }
    LogEventType? LogEventType { get; set; }
    User? UserEntity { get; }
    Tenant? TenantEntity { get; }
    TenantUser? TenantUserEntity { get;  }
    Role? RoleEntity { get; }
    
    void Inject(UserExecutionContext context);
    
    UserExecutionContext ToExecutionContext(); 
}
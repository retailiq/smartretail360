using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Shared.Contexts.User;

public class UserExecutionContext
{
    public Guid? UserId { get; init; }
    public Guid? TenantId { get; init; }
    public Guid? RoleId { get; init; }
    public string? TraceId { get; init; }
    public string? Locale { get; init; }
    public string? Module { get; init; }
    public string? Email { get; init; }
    public string? ErrorStack { get; init; }
    public string? IpAddress { get; init; }
    public string? Action { get; init; }
    public string? RoleName { get; init; }
    public string? LogId { get; init; }
    public string? UserName { get; init; }
    public LogEventType? LogEventType { get; init; }
    
    public Domain.Entities.User? UserEntity { get; set; }
    public TenantUser? TenantUserEntity { get; set; }
    public Tenant? TenantEntity { get; set; }
    public Role? RoleEntity { get; set; }
}
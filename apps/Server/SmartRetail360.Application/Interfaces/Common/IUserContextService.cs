using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Interfaces.Common;

public interface IUserContextService
{
    Guid? UserId { get; }
    Guid? TenantId { get; }
    Guid? RoleId { get; }
    string? TraceId { get; }
    string? Locale { get; }
    string IpAddress { get; }
    string? Module { get; set; }
    string? ClientEmail { get; }
    AccountType? AccountType { get; }
    
    void LogAllContext();
}
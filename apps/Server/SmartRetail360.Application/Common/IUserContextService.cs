using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Common;

public interface IUserContextService
{
    Guid? UserId { get; set; }
    Guid? TenantId { get; set; }
    Guid? RoleId { get; set; }
    string? TraceId { get; set; }
    string? Locale { get; set; }
    string IpAddress { get; set; }
    string? Module { get; set; }
    string? ClientEmail { get; set; }
    AccountType? AccountType { get; set; }
    string? ErrorStack { get; set; } 
    string? Action { get; set; }
    
    void Inject(
        Guid? userId = null,
        Guid? tenantId = null,
        Guid? roleId = null,
        string? traceId = null,
        string? locale = null,
        string? module = null,
        string? clientEmail = null,
        AccountType? accountType = null,
        string? errorStack = null,
        string? ipAddress = null,
        string? action = null
    );
}
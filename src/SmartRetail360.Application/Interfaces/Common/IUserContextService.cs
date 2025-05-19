// using SmartRetail360.Shared.Enums;
//
// namespace SmartRetail360.Application.Interfaces.Common;
//
// public interface IUserContextService
// {
//     Guid? UserId { get; }
//     Guid? TenantId { get; }
//     Guid? RoleId { get; }
//     string? TraceId { get; }
//     string? Locale { get; }
//     string IpAddress { get; }
//     string? Module { get; set; }
//     string? ClientEmail { get; }
//     AccountType? AccountType { get; }
//     
//     void LogAllContext();
// }

using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Interfaces.Common;

public interface IUserContextService
{
    Guid? UserId { get; set; }
    Guid? TenantId { get; set; }
    Guid? RoleId { get; set; }
    string? TraceId { get; set; }
    string? Locale { get; set; }
    string IpAddress { get; } // 这个仍然只能 get（由 HttpContext 计算）
    string? Module { get; set; }
    string? ClientEmail { get; set; }
    AccountType? AccountType { get; set; }

    void LogAllContext();
}
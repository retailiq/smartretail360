namespace SmartRetail360.Application.Interfaces.Logging;

public interface ILogContextAccessor
{
    string? TraceId { get; }
    string? ClientEmail { get; }
    string? Locale { get; }
    Guid? UserId { get; }
    Guid? TenantId { get; }
    Guid? RoleId { get; }
    string? Module { get; }
    string? IpAddress { get; }
    string? AccountType { get; }
}
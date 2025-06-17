namespace SmartRetail360.Logging.Interfaces;

public interface ILogContextAccessor
{
    string? TraceId { get; }
    string? Email { get; }
    string? Locale { get; }
    Guid? UserId { get; }
    Guid? TenantId { get; }
    Guid? RoleId { get; }
    string? Module { get; }
    string? IpAddress { get; }
    string? ErrorStack { get; }
    string? Action { get; }
    string? RoleName { get; }
    string? LogId { get; }
    string? Env { get; }
}
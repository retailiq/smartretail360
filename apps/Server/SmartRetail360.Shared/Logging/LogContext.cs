namespace SmartRetail360.Shared.Logging;

public class LogContext
{
    public string LogId { get; set; } = Guid.NewGuid().ToString();

    // basic info
    public string? Email { get; init; }
    public string? Reason { get; init; }
    public string? ErrorStack { get; init; }

    // action info
    public string? Action { get; set; }
    public bool IsSuccess { get; set; }
    public string? LogCategory { get; set; } // Application / Behavior / Security / System

    // trace info
    public string? TraceId { get; set; }
    public string? SpanId { get; set; }

    // user context
    public Guid? UserId { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? RoleId { get; set; }
    public string? ClientEmail { get; set; }
    public string? AccountType { get; set; }

    // environment info
    public string? Locale { get; set; }
    public string? IpAddress { get; set; }
    public string? Module { get; set; }
}
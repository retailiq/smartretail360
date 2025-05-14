using Microsoft.Extensions.Logging;

namespace SmartRetail360.Shared.Logging;

public class LogWriteRule
{
    public bool WriteAudit { get; set; } = true;
    public bool WriteSystemLog { get; set; } = false;
    public string? SystemLogAction { get; set; }
    public string? SystemLogCategory { get; set; }
    public LogLevel SystemLogLevel { get; set; } = LogLevel.Information;
}
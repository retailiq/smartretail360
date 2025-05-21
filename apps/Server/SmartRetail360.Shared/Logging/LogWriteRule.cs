using Microsoft.Extensions.Logging;

namespace SmartRetail360.Shared.Logging;

public class LogWriteRule
{
    public bool WriteAudit { get; set; } = true;
    public bool WriteSystemLog { get; set; } = true;
    public bool SendToSentry { get; set; } = false;
    public LogLevel LogLevel { get; set; } = LogLevel.Information;
    public string? LogAction { get; set; }      
    public bool? IsSuccess { get; set; }      
    public string? LogCategory { get; set; }  
}
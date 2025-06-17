using SmartRetail360.Shared.Logging;

namespace SmartRetail360.Logging.Interfaces;

public interface ILogWriter
{
    Task WriteAsync(LogContext context, LogWriteRule rule);
}
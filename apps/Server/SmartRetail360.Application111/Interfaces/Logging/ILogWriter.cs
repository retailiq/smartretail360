using SmartRetail360.Shared.Logging;

namespace SmartRetail360.Application.Interfaces.Common;

public interface ILogWriter
{
    Task WriteAsync(LogContext context, LogWriteRule rule);
}
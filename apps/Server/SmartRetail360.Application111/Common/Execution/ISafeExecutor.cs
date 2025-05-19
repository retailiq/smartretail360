using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Common.Execution;

public interface ISafeExecutor
{
    Task<SafeExecutionResult> ExecuteAsync(
        Func<Task> action,
        LogEventType logEvent,
        string reasonOnFailure,
        int errorCode,
        string? email = null,
        Guid ? tenantId = null,
        Guid ? userId = null);

    Task<SafeExecutionResult<T>> ExecuteAsync<T>(
        Func<Task<T>> action,
        LogEventType logEvent,
        string reasonOnFailure,
        int errorCode,
        string? email = null,
        Guid ? tenantId = null,
        Guid ? userId = null);
}
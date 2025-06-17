using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Execution;

public interface IGuardChecker
{
    IGuardChecker Check(Func<bool> condition, LogEventType logEvent, string reason, int errorCode);
    IGuardChecker CheckAsync(Func<Task<bool>> condition, LogEventType logEvent, string reason, int errorCode);
    Task<ApiResponse<object>?> ValidateAsync();
}
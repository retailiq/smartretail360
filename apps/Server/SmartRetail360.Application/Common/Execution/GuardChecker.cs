using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Application.Common.Execution;

public class GuardChecker
{
    private readonly List<Func<Task<ApiResponse<object>?>>> _checks = new();
    private readonly ILogDispatcher _logDispatcher;
    private readonly IUserContextService _userContext;
    private readonly MessageLocalizer _localizer;

    public GuardChecker(
        ILogDispatcher logDispatcher,
        IUserContextService userContext,
        MessageLocalizer localizer)
    {
        _logDispatcher = logDispatcher;
        _userContext = userContext;
        _localizer = localizer;
    }

    public GuardChecker Check(Func<bool> condition, LogEventType logEvent, string reason, int errorCode)
    {
        _checks.Add(async () =>
        {
            if (condition())
            {
                await _logDispatcher.Dispatch(
                    logEvent,
                    reason: reason
                );

                return ApiResponse<object>.Fail(
                    errorCode,
                    _localizer.GetErrorMessage(errorCode),
                    _userContext.TraceId
                );
            }

            return null;
        });

        return this;
    }

    public GuardChecker CheckAsync(Func<Task<bool>> condition, LogEventType logEvent, string reason, int errorCode)
    {
        _checks.Add(async () =>
        {
            if (await condition())
            {
                await _logDispatcher.Dispatch(
                    logEvent,
                    reason: reason
                );

                return ApiResponse<object>.Fail(
                    errorCode,
                    _localizer.GetErrorMessage(errorCode),
                    _userContext.TraceId
                );
            }

            return null;
        });

        return this;
    }
    
    public async Task<ApiResponse<object>?> ValidateAsync()
    {
        foreach (var check in _checks)
        {
            var result = await check();
            if (result != null)
                return result;
        }

        return null; // 所有条件通过，继续往下走
    }
}
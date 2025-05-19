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

    private string? _email;
    private Guid? _tenantId;
    private Guid ? _userId;

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
                    email: _email ?? _userContext.ClientEmail,
                    tenantId: _tenantId,
                    userId: _userId,
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
                    email: _email ?? _userContext.ClientEmail,
                    tenantId: _tenantId,
                    userId: _userId,
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

    public GuardChecker WithEmail(string? email)
    {
        _email = email;
        return this;
    }

    public GuardChecker WithTenantId(Guid? tenantId)
    {
        _tenantId = tenantId;
        return this;
    }
    
    public GuardChecker WithUserId(Guid? userId)
    {
        _userId = userId;
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
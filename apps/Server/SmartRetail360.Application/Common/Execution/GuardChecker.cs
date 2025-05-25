using SmartRetail360.Application.Common.UserContext;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Responses;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Common.Execution
{
    public class GuardChecker : IGuardChecker
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

        public IGuardChecker Check(Func<bool> condition, LogEventType logEvent, string reason, int errorCode)
        {
            _checks.Add(async () =>
            {
                if (condition())
                {
                    await _logDispatcher.Dispatch(logEvent, reason);
                    return ApiResponse<object>.Fail(errorCode, _localizer.GetErrorMessage(errorCode), _userContext.TraceId);
                }
                return null;
            });
            return this;
        }

        public IGuardChecker CheckAsync(Func<Task<bool>> condition, LogEventType logEvent, string reason, int errorCode)
        {
            _checks.Add(async () =>
            {
                if (await condition())
                {
                    await _logDispatcher.Dispatch(logEvent, reason);
                    return ApiResponse<object>.Fail(errorCode, _localizer.GetErrorMessage(errorCode), _userContext.TraceId);
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
            return null;
        }
    }
}
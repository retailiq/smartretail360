using Microsoft.EntityFrameworkCore;
using Npgsql;
using SmartRetail360.Application.Common;
using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Logging;

namespace SmartRetail360.Infrastructure.Common.Execution;

public class SafeExecutor : ISafeExecutor
{
    private readonly Lazy<ILogDispatcher> _logDispatcher;
    private readonly IUserContextService _userContext;
    private readonly MessageLocalizer _localizer;

    public SafeExecutor(
        Lazy<ILogDispatcher> logDispatcher,
        IUserContextService userContext,
        MessageLocalizer localizer)
    {
        _logDispatcher = logDispatcher;
        _userContext = userContext;
        _localizer = localizer;
    }

    public async Task<SafeExecutionResult> ExecuteAsync(
        Func<Task> action,
        LogEventType logEvent,
        string reasonOnFailure,
        int errorCode)
    {
        try
        {
            await action();
            return SafeExecutionResult.Success(traceId: _userContext.TraceId);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            _userContext.Inject(
                errorStack: pgEx.ToString()
            );
            await _logDispatcher.Value.Dispatch(logEvent, reason: reasonOnFailure);
            return SafeExecutionResult.Fail(errorCode, _localizer.GetErrorMessage(errorCode), _userContext.TraceId);
        }
        catch (Exception ex)
        {
            _userContext.Inject(
                errorStack: ex.ToString()
            );
            await _logDispatcher.Value.Dispatch(logEvent, reason: reasonOnFailure);
            return SafeExecutionResult.Fail(errorCode, _localizer.GetErrorMessage(errorCode), _userContext.TraceId);
        }
    }

    public async Task<SafeExecutionResult<T>> ExecuteAsync<T>(
        Func<Task<T>> action,
        LogEventType logEvent,
        string reasonOnFailure,
        int errorCode)
    {
        try
        {
            var result = await action();
            return SafeExecutionResult<T>.Success(result, traceId: _userContext.TraceId);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            _userContext.Inject(
                errorStack: pgEx.ToString()
            );
            await _logDispatcher.Value.Dispatch(logEvent, reason: reasonOnFailure);
            return SafeExecutionResult<T>.Fail(errorCode, _localizer.GetErrorMessage(errorCode), _userContext.TraceId);
        }
        catch (Exception ex)
        {
            _userContext.Inject(
                errorStack: ex.ToString()
            );
            await _logDispatcher.Value.Dispatch(logEvent, reason: reasonOnFailure);
            return SafeExecutionResult<T>.Fail(errorCode, _localizer.GetErrorMessage(errorCode), _userContext.TraceId);
        }
    }
}
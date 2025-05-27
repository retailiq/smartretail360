using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Auth.UserLogin;

public class LoginGuardChecks
{
    private readonly LoginContext _ctx;

    public LoginGuardChecks(LoginContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ApiResponse<LoginResponse>?> CheckRedisLocksAsync()
    {
        var lockAcquired = await _ctx._dep.RedisOperation.AcquireLockAsync(_ctx.LockKey,
            TimeSpan.FromSeconds(_ctx._dep.AppOptions.UserLoginLockTtlSeconds));
        var loginLockCheck = await _ctx._dep.GuardChecker
            .Check(() => !lockAcquired, LogEventType.UserLoginFailure, LogReasons.LockNotAcquired,
                ErrorCodes.DuplicateLoginAttempt)
            .ValidateAsync();
        if (loginLockCheck != null)
            return loginLockCheck.To<LoginResponse>();

        var isLoginLocked = await _ctx._dep.RedisOperation.IsUserLoginLockedAsync(_ctx.SecurityKey);
        var accountStatusLockCheck = await _ctx._dep.GuardChecker
            .Check(() => isLoginLocked, LogEventType.UserLoginFailure, LogReasons.AccountLockedDueToLoginFailures,
                ErrorCodes.AccountLocked)
            .ValidateAsync();
        if (accountStatusLockCheck != null)
            return accountStatusLockCheck.To<LoginResponse>();

        return null;
    }

    public async Task<ApiResponse<LoginResponse>?> CheckPasswordAndActivationAsync()
    {
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(_ctx.Request.Password, _ctx.User!.PasswordHash);
        var guardResult = await _ctx._dep.GuardChecker
            .Check(() => !isPasswordValid, LogEventType.UserLoginFailure,
                LogReasons.PasswordEmailMismatch, ErrorCodes.PasswordEmailMismatch)
            .ValidateAsync();
        if (guardResult != null)
        {
            var count = await _ctx._dep.RedisOperation.IncrementUserLoginFailureAsync(_ctx.FailKey, _ctx.LockKey);
            if (count >= 3)
            {
                _ctx.User.StatusEnum = AccountStatus.Locked;
                _ctx.User.DeactivationReasonEnum = AccountBanReason.LoginFailureLimit;
                await _ctx._dep.SafeExecutor.ExecuteAsync(
                    async () => { await _ctx._dep.Db.SaveChangesAsync(); },
                    LogEventType.DatabaseError,
                    LogReasons.DatabaseSaveFailed,
                    ErrorCodes.DatabaseUnavailable
                );
                return ApiResponse<LoginResponse>.Fail(
                    code: ErrorCodes.AccountLocked,
                    details: _ctx._dep.Localizer.GetErrorMessage(ErrorCodes.AccountLocked),
                    traceId: _ctx.TraceId
                );
            }

            return guardResult.To(new LoginResponse { LoginFailureCount = count });
        }

        var activationCheck = await _ctx._dep.GuardChecker
            .Check(() => _ctx.User.StatusEnum == AccountStatus.PendingVerification,
                LogEventType.UserLoginFailure, LogReasons.AccountNotActivated,
                ErrorCodes.AccountNotActivated)
            .ValidateAsync();
        if (activationCheck != null)
        {
            var (tokenList, tokenListError) = await _ctx._dep.AccountSupport.GetActivationTokenListAsync(_ctx.User.Id);
            if (tokenListError != null)
                return tokenListError.To<LoginResponse>();

            var tokenListCheckResult = await _ctx._dep.GuardChecker
                .Check(() => tokenList!.Count == 0,
                    LogEventType.UserLoginFailure, LogReasons.TokenNotFound,
                    ErrorCodes.TokenNotFound)
                .ValidateAsync();
            if (tokenListCheckResult != null)
                return tokenListCheckResult.To<LoginResponse>();

            var latestToken = tokenList!.FirstOrDefault();
            var isPending = latestToken!.ExpiresAt < DateTime.UtcNow &&
                            latestToken.StatusEnum == ActivationTokenStatus.Pending;

            return activationCheck.To(new LoginResponse { ShouldShowResendButton = isPending });
        }

        var accountStatusResult = await _ctx._dep.GuardChecker
            .Check(() => _ctx.User.StatusEnum == AccountStatus.Locked,
                LogEventType.UserLoginFailure, LogReasons.AccountLocked,
                ErrorCodes.AccountLocked)
            .Check(() => _ctx.User.StatusEnum == AccountStatus.Suspended,
                LogEventType.UserLoginFailure, LogReasons.AccountSuspended,
                ErrorCodes.AccountSuspended)
            .Check(() => _ctx.User.StatusEnum == AccountStatus.Deleted,
                LogEventType.UserLoginFailure, LogReasons.AccountDeleted,
                ErrorCodes.AccountDeleted)
            .Check(() => _ctx.User.StatusEnum == AccountStatus.Banned,
                LogEventType.UserLoginFailure, LogReasons.AccountBanned,
                ErrorCodes.AccountBanned)
            .ValidateAsync();
        if (accountStatusResult != null)
            return accountStatusResult.To<LoginResponse>();

        return null;
    }
}
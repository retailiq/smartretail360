using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Infrastructure.Services.Auth.Login.Interfaces;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Auth.Login.Shared;

public class LoginGuardCheckers
{
    private readonly ILoginContextBase _ctx;

    public LoginGuardCheckers(ILoginContextBase ctx)
    {
        _ctx = ctx;
    }

    public async Task<ApiResponse<LoginResponse>?> CheckRedisLocksAsync()
    {
        var lockAcquired = await _ctx.Dep.RedisOperation.AcquireUserLoginLockAsync(_ctx.User!.Email.ToLower());
        
        await _ctx.Dep.RedisOperation.ResetUserLoginFailuresAsync(_ctx.User.Email);
        
        var loginLockCheck = await _ctx.Dep.GuardChecker
            .Check(() => !lockAcquired, LogEventType.LoginFailure, LogReasons.LockNotAcquired,
                ErrorCodes.DuplicateLoginAttempt)
            .ValidateAsync();
        if (loginLockCheck != null)
            return loginLockCheck.To<LoginResponse>();

        var isLoginLocked = await _ctx.Dep.RedisOperation.IsUserLoginLockedAsync(_ctx.User!.Email);
        var accountStatusLockCheck = await _ctx.Dep.GuardChecker
            .Check(() => isLoginLocked, LogEventType.LoginFailure, LogReasons.AccountLockedDueToLoginFailures,
                ErrorCodes.AccountLocked)
            .ValidateAsync();
        if (accountStatusLockCheck != null)
            return accountStatusLockCheck.To<LoginResponse>();

        return null;
    }

    public async Task<ApiResponse<LoginResponse>?> CheckAccountStatusAsync()
    {
        var activationCheck = await _ctx.Dep.GuardChecker
            .Check(() => _ctx.User!.StatusEnum == AccountStatus.PendingVerification,
                LogEventType.LoginFailure, LogReasons.AccountNotActivated,
                ErrorCodes.AccountNotActivated)
            .ValidateAsync();
        if (activationCheck != null)
        {
            var (tokenList, tokenListError) = await _ctx.Dep.AccountSupport.GetActivationTokenListAsync(_ctx.User!.Id);
            if (tokenListError != null)
                return tokenListError.To<LoginResponse>();

            var tokenListCheckResult = await _ctx.Dep.GuardChecker
                .Check(() => tokenList!.Count == 0,
                    LogEventType.LoginFailure, LogReasons.TokenNotFound,
                    ErrorCodes.TokenNotFound)
                .ValidateAsync();
            if (tokenListCheckResult != null)
                return tokenListCheckResult.To<LoginResponse>();

            var latestToken = tokenList!.FirstOrDefault();
            var isPending = latestToken!.ExpiresAt < DateTime.UtcNow &&
                            latestToken.StatusEnum == ActivationTokenStatus.Pending;

            return activationCheck.To(new LoginResponse { ShouldShowResendButton = isPending });
        }

        var accountStatusResult = await _ctx.Dep.GuardChecker
            .Check(() => _ctx.User!.StatusEnum == AccountStatus.Locked,
                LogEventType.LoginFailure, LogReasons.AccountLocked,
                ErrorCodes.AccountLocked)
            .Check(() => _ctx.User!.StatusEnum == AccountStatus.Suspended,
                LogEventType.LoginFailure, LogReasons.AccountSuspended,
                ErrorCodes.AccountSuspended)
            .Check(() => _ctx.User!.StatusEnum == AccountStatus.Deleted,
                LogEventType.LoginFailure, LogReasons.AccountDeleted,
                ErrorCodes.AccountDeleted)
            .Check(() => _ctx.User!.StatusEnum == AccountStatus.Banned,
                LogEventType.LoginFailure, LogReasons.AccountBanned,
                ErrorCodes.AccountBanned)
            .ValidateAsync();
        if (accountStatusResult != null)
            return accountStatusResult.To<LoginResponse>();

        return null;
    }
}
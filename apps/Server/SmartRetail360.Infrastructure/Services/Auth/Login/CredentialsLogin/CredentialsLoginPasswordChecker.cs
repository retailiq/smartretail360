using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Auth.Login.CredentialsLogin;

public class CredentialsLoginPasswordChecker
{
    private readonly CredentialsLoginContext _ctx;

    public CredentialsLoginPasswordChecker(CredentialsLoginContext ctx)
    {
        _ctx = ctx;
    }
    
    public async Task<ApiResponse<LoginResponse>?> CheckPasswordAsync()
    {
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(_ctx.Request.Password, _ctx.User!.PasswordHash);
        var guardResult = await _ctx.Dep.GuardChecker
            .Check(() => !isPasswordValid, LogEventType.UserLoginFailure,
                LogReasons.PasswordEmailMismatch, ErrorCodes.PasswordEmailMismatch)
            .ValidateAsync();
        if (guardResult != null)
        {
            var count = await _ctx.Dep.RedisOperation.IncrementUserLoginFailureAsync(_ctx.FailKey, _ctx.LockKey);
            if (count >= 3)
            {
                _ctx.User.StatusEnum = AccountStatus.Locked;
                _ctx.User.DeactivationReasonEnum = AccountBanReason.LoginFailureLimit;
                await _ctx.Dep.SafeExecutor.ExecuteAsync(
                    async () => { await _ctx.Dep.Db.SaveChangesAsync(); },
                    LogEventType.DatabaseError,
                    LogReasons.DatabaseSaveFailed,
                    ErrorCodes.DatabaseUnavailable
                );
                return ApiResponse<LoginResponse>.Fail(
                    code: ErrorCodes.AccountLocked,
                    details: _ctx.Dep.Localizer.GetErrorMessage(ErrorCodes.AccountLocked),
                    traceId: _ctx.TraceId
                );
            }

            return guardResult.To(new LoginResponse { LoginFailureCount = count });
        }

        return null;
    }
}
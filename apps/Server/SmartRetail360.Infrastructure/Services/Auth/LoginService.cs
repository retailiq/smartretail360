using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Infrastructure.Services.Auth.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Context;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Redis;
using SmartRetail360.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Infrastructure.Services.Auth;

public class LoginService : ILoginService
{
    private readonly LoginDependencies _dep;

    public LoginService(LoginDependencies dep)
    {
        _dep = dep;
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        _dep.UserContext.Inject(new UserExecutionContext
        {
            Email = request.Email,
            Action = LogActions.UserLogin
        });

        var traceId = _dep.UserContext.TraceId;
        var lockKey = RedisKeys.UserLoginLock(request.Email.ToLower());
        var failKey = RedisKeys.UserLoginFailures(request.Email);
        var securityKey = RedisKeys.UserLoginSecurityLock(request.Email);

        var lockAcquired = await _dep.RedisOperation.AcquireLockAsync(lockKey,
            TimeSpan.FromSeconds(_dep.AppOptions.UserLoginLockTtlSeconds));
        var loginLockCheck = await _dep.GuardChecker
            .Check(() => !lockAcquired, LogEventType.UserLoginFailure, LogReasons.LockNotAcquired,
                ErrorCodes.DuplicateLoginAttempt)
            .ValidateAsync();
        if (loginLockCheck != null)
            return loginLockCheck.To<LoginResponse>();

        var isLoginLocked = await _dep.RedisOperation.IsUserLoginLockedAsync(securityKey);
        var accountStatusLockCheck = await _dep.GuardChecker
            .Check(() => isLoginLocked, LogEventType.UserLoginFailure, LogReasons.AccountLockedDueToLoginFailures,
                ErrorCodes.AccountLocked)
            .ValidateAsync();
        if (accountStatusLockCheck != null)
            return accountStatusLockCheck.To<LoginResponse>();

        try
        {
            var (user, userError) = await _dep.PlatformContext.GetUserByEmailAsync(request.Email);
            if (userError != null)
                return userError.To<LoginResponse>();

            var userCheckResult = await _dep.GuardChecker
                .Check(() => user == null, LogEventType.UserLoginFailure,
                    LogReasons.AccountNotFound, ErrorCodes.AccountNotFound)
                .ValidateAsync();
            if (userCheckResult != null)
                return userCheckResult.To<LoginResponse>();

            _dep.UserContext.Inject(new UserExecutionContext { UserId = user!.Id });

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            var guardResult = await _dep.GuardChecker
                .Check(() => !isPasswordValid, LogEventType.UserLoginFailure,
                    LogReasons.PasswordEmailMismatch, ErrorCodes.PasswordEmailMismatch)
                .ValidateAsync();
            if (guardResult != null)
            {
                var count = await _dep.RedisOperation.IncrementUserLoginFailureAsync(failKey, lockKey);
                if (count >= 3)
                {
                    user.StatusEnum = AccountStatus.Locked;
                    await _dep.Db.SaveChangesAsync();

                    return ApiResponse<LoginResponse>.Fail(
                        code: ErrorCodes.AccountLocked,
                        details: _dep.Localizer.GetErrorMessage(ErrorCodes.AccountLocked),
                        traceId: traceId
                    );
                }

                return guardResult.To(new LoginResponse
                {
                    LoginFailureCount = count
                });
            }

            var accountActivationResult = await _dep.GuardChecker
                .Check(() => user.StatusEnum == AccountStatus.PendingVerification,
                    LogEventType.UserLoginFailure, LogReasons.AccountNotActivated,
                    ErrorCodes.AccountNotActivated)
                .ValidateAsync();
            if (accountActivationResult != null)
            {
                var tokenListResult = await _dep.SafeExecutor.ExecuteAsync(
                    () => _dep.Db.AccountActivationTokens
                        .Where(t => t.UserId == user.Id)
                        .OrderByDescending(t => t.CreatedAt)
                        .ToListAsync(),
                    LogEventType.UserLoginFailure,
                    LogReasons.DatabaseRetrievalFailed,
                    ErrorCodes.DatabaseUnavailable
                );
                if (!tokenListResult.IsSuccess)
                    return tokenListResult.ToObjectResponse().To<LoginResponse>();
                var tokenList = tokenListResult.Response.Data!;
                var latestToken = tokenList.FirstOrDefault();
                var isPending = latestToken!.ExpiresAt < DateTime.UtcNow &&
                                latestToken.StatusEnum == ActivationTokenStatus.Pending;
                return accountActivationResult.To(new LoginResponse
                {
                    ShouldShowResendButton = isPending
                });
            }

            var accountStatusResult = await _dep.GuardChecker
                .Check(() => user.StatusEnum == AccountStatus.Locked,
                    LogEventType.UserLoginFailure, LogReasons.AccountLocked,
                    ErrorCodes.AccountLocked)
                .ValidateAsync();
            if (accountStatusResult != null)
                return accountStatusResult.To<LoginResponse>();

            var tenantUsers = await _dep.Db.TenantUsers
                .Where(tu => tu.UserId == user.Id)
                .Include(tu => tu.Tenant)
                .Include(tu => tu.Role)
                .ToListAsync();

            var inactiveTenants = tenantUsers
                .Where(tu => !tu.Tenant!.IsActive)
                .ToList();
            var tenantStatusResult = await _dep.GuardChecker
                .Check(() => inactiveTenants.Count == tenantUsers.Count,
                    LogEventType.UserLoginFailure, LogReasons.AllTenantsDisabled,
                    ErrorCodes.AllTenantsDisabled)
                .ValidateAsync();
            if (tenantStatusResult != null)
                return tenantStatusResult.To<LoginResponse>();

            var roleIds = tenantUsers.Select(tu => tu.RoleId).Distinct().ToList();
            var roles = await _dep.RedisOperation.GetSystemRolesByIdsAsync(roleIds);
            var roleResult = await _dep.GuardChecker
                .Check(() => roles.Count == 0,
                    LogEventType.UserLoginFailure, LogReasons.RoleListNotFound,
                    ErrorCodes.InternalServerError)
                .ValidateAsync();
            if (roleResult != null)
                return roleResult.To<LoginResponse>();

            var candidates = tenantUsers.Select(tu => new TenantLoginCandidate
            {
                TenantId = tu.TenantId.ToString(),
                TenantName = tu.Tenant!.Name ?? GeneralConstants.Unknown,
                LogoUrl = tu.Tenant.LogoUrl,
                RoleId = tu.RoleId.ToString(),
                RoleName = RoleHelper.ToPascalCaseName(tu.Role!.Name),
                IsDefault = tu.IsDefault,
                IsActive = tu.IsActive
            }).ToList();

            var shouldChooseTenant = !candidates.Any(c => c.IsDefault);

            var loginResponse = new LoginResponse
            {
                UserId = user.Id.ToString(),
                Email = user.Email,
                ShouldChooseTenant = shouldChooseTenant,
                TenantOptions = candidates
            };
            
            await _dep.SafeExecutor.ExecuteAsync(
                async () =>
                {
                    await _dep.RedisOperation.ResetUserLoginFailuresAsync(failKey, lockKey);
                },
                LogEventType.GeneralError,
                LogReasons.DatabaseSaveFailed,
                ErrorCodes.DatabaseUnavailable
            );
            
            return ApiResponse<LoginResponse>.Ok(
                loginResponse,
                _dep.Localizer.GetLocalizedText(LocalizedTextKey.UserLoginSuccess),
                traceId
            );
        }
        finally
        {
            await _dep.RedisOperation.ReleaseLockAsync(lockKey);
        }
    }
}
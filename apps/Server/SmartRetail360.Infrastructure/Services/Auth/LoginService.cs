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
        var lockAcquired = await _dep.RedisOperation.AcquireLockAsync(lockKey,
            TimeSpan.FromSeconds(_dep.AppOptions.UserLoginLockTtlSeconds));
        var lockCheck = await _dep.GuardChecker
            .Check(() => !lockAcquired, LogEventType.UserLoginFailure, LogReasons.LockNotAcquired,
                ErrorCodes.DuplicateLoginAttempt)
            .ValidateAsync();
        if (lockCheck != null)
            return lockCheck.To<LoginResponse>();

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
                return ApiResponse<LoginResponse>.Fail(
                    code: ErrorCodes.AccountNotActivated,
                    details: _dep.Localizer.GetErrorMessage(ErrorCodes.AccountNotActivated),
                    traceId: traceId,
                    data: new LoginResponse
                    {
                        ShouldShowResendButton = isPending,
                    }
                );
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            var guardResult = await _dep.GuardChecker
                .Check(() => !isPasswordValid, LogEventType.UserLoginFailure,
                    LogReasons.PasswordEmailMismatch, ErrorCodes.PasswordEmailMismatch)
                .ValidateAsync();
            if (guardResult != null)
                return guardResult.To<LoginResponse>();
                        
            var tenantUsers = await _dep.Db.TenantUsers
                .Where(tu => tu.UserId == user.Id)
                .Include(tu => tu.Tenant)
                .Include(tu => tu.Role)
                .ToListAsync();
            var roleIds = tenantUsers!.Select(tu => tu.RoleId).Distinct().ToList();
            
            var role = await _dep.RedisOperation.GetSystemRolesByIdsAsync(roleIds);
            var roleResult = await _dep.GuardChecker
                .Check(() => role.Count == 0,
                    LogEventType.UserLoginFailure, LogReasons.DatabaseRetrievalFailed,
                    ErrorCodes.DatabaseUnavailable)
                .ValidateAsync();
            if (roleResult != null)
                return roleResult.To<LoginResponse>();
            
            var candidates = tenantUsers.Select(tu => new TenantLoginCandidate
            {
                TenantId = tu.TenantId.ToString(),
                TenantName = tu.Tenant?.Name ?? GeneralConstants.Unknown,
                LogoUrl = tu.Tenant?.LogoUrl,
                RoleId = tu.RoleId.ToString(),
                RoleName = RoleHelper.ToPascalCaseName(tu.Role?.Name ?? GeneralConstants.Unknown),
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
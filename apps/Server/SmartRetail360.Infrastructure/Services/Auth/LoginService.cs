using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Infrastructure.Services.Auth.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Context;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;
using SmartRetail360.Shared.Redis;
using SmartRetail360.Shared.Responses;
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

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            var guardResult = await _dep.GuardChecker
                .Check(() => !isPasswordValid, LogEventType.UserLoginFailure,
                    LogReasons.PasswordEmailMismatch, ErrorCodes.PasswordEmailMismatch)
                .Check(() => user.StatusEnum == AccountStatus.PendingVerification,
                    LogEventType.UserLoginFailure, LogReasons.AccountNotActivated,
                    ErrorCodes.AccountNotActivated)
                .ValidateAsync();
            if (guardResult != null)
                return guardResult.To<LoginResponse>();

            var (tenantUser, tenantUserError) = await _dep.PlatformContext.GetTenantUserAsync(user.Id);
            if (tenantUserError != null)
                return tenantUserError.To<LoginResponse>();

            var accessToken = _dep.JwtTokenGenerator.GenerateToken(
                userId: user.Id.ToString(),
                email: user.Email,
                name: user.Name,
                tenantId: tenantUser!.TenantId.ToString(),
                roleId: tenantUser.RoleId.ToString(),
                locale: user.Locale.GetEnumMemberValue(),
                traceId: _dep.UserContext.TraceId
            );

            var refreshToken = TokenHelper.GenerateRefreshToken();

            return ApiResponse<LoginResponse>.Ok(
                new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresIn = _dep.AppOptions.JwtExpirySeconds,
                    User = new AuthUserInfo
                    {
                        UserId = user.Id.ToString(),
                        Email = user.Email,
                        Name = user.Name,
                        Locale = user.Locale.GetEnumMemberValue(),
                        TenantId = tenantUser.TenantId.ToString(),
                        RoleId = tenantUser.RoleId.ToString(),
                        AvatarUrl = user.AvatarUrl ?? string.Empty,
                        IsFirstLogin = user.IsFirstLogin,
                        Permissions = new List<string>()
                        // Permissions = await _dep.PlatformContext.GetUserPermissionsAsync(user.Id, tenantUser.RoleId)
                    }
                },
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
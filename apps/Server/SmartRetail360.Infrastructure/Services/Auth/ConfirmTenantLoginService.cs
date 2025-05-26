using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Infrastructure.Services.Auth.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Auth;

public class ConfirmTenantLoginService : IConfirmTenantLoginService
{
    private readonly ConfirmTenantLoginDependencies _dep;

    public ConfirmTenantLoginService(ConfirmTenantLoginDependencies dep)
    {
        _dep = dep;
    }

    public async Task<ApiResponse<ConfirmTenantLoginResponse>> ConfirmTenantLoginAsync(
        ConfirmTenantLoginRequest request)
    {
        var traceId = _dep.UserContext.TraceId;
        var refreshTokenExpiryDays = request.IsStaySignedIn == true
            ? _dep.AppOptions.RefreshTokenExpiryDaysWhenStaySignedIn
            : _dep.AppOptions.RefreshTokenExpiryDaysDefault;

        var tenantUser = await _dep.Db.TenantUsers
            .Include(tu => tu.Tenant)
            .Include(tu => tu.Role)
            .Include(tu => tu.User)
            .FirstOrDefaultAsync(tu =>
                tu.UserId == request.UserId &&
                tu.TenantId == request.TenantId &&
                tu.IsActive);

        var accountCheckResult = await _dep.GuardChecker
            .Check(() => tenantUser == null,
                LogEventType.UserLoginFailure, LogReasons.TenantUserDisabled,
                ErrorCodes.TenantUserDisabled)
            .Check(() => tenantUser is { User: null },
                LogEventType.UserLoginFailure, LogReasons.AccountNotFound,
                ErrorCodes.AccountNotFound)
            .Check(() => tenantUser is { Tenant: null },
                LogEventType.UserLoginFailure, LogReasons.TenantNotFound,
                ErrorCodes.TenantNotFound)
            .Check(() => !tenantUser!.Tenant!.IsActive,
                LogEventType.UserLoginFailure, LogReasons.TenantDisabled,
                ErrorCodes.TenantDisabled)
            .ValidateAsync();
        if (accountCheckResult != null)
            return accountCheckResult.To<ConfirmTenantLoginResponse>();

        var isFirstLogin = tenantUser!.User!.IsFirstLogin;

        var accessToken = _dep.AccessTokenGenerator.GenerateToken(
            userId: tenantUser.Id.ToString(),
            email: tenantUser.User.Email,
            name: tenantUser.User.Name,
            tenantId: tenantUser.TenantId.ToString(),
            roleId: tenantUser.RoleId.ToString(),
            locale: tenantUser.User.Locale.GetEnumMemberValue(),
            traceId: _dep.UserContext.TraceId
        );

        var refreshToken = await _dep.RefreshTokenService.CreateRefreshTokenAsync(
            tenantUser.UserId,
            tenantUser.TenantId,
            _dep.UserContext.IpAddress,
            refreshTokenExpiryDays
        );

        tenantUser.User.LastLoginAt = DateTime.UtcNow;
        tenantUser.User.TraceId = traceId;
        tenantUser.User.IsFirstLogin = false;

        var saveResult = await _dep.SafeExecutor.ExecuteAsync(
            async () =>
            {
                await _dep.Db.SaveChangesAsync();
            },
            LogEventType.ConfirmTenantLoginFailure,
            LogReasons.DatabaseSaveFailed,
            ErrorCodes.DatabaseUnavailable
        );
        if (!saveResult.IsSuccess)
            return saveResult.ToObjectResponse().To<ConfirmTenantLoginResponse>();

        _dep.HttpContext.Response.Cookies.Append(GeneralConstants.Sr360RefreshToken, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Domain = _dep.AppOptions.CookieDomain,
            SameSite = SameSiteMode.Strict,
            Path = _dep.AppOptions.RefreshTokenPath,
            Expires = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(refreshTokenExpiryDays))
        });

        return ApiResponse<ConfirmTenantLoginResponse>.Ok(
            new ConfirmTenantLoginResponse
            {
                AccessToken = accessToken,
                ExpiresIn = _dep.AppOptions.AccessTokenExpirySeconds,
                User = new AuthUserInfo
                {
                    UserId = tenantUser.UserId.ToString(),
                    Email = tenantUser.User.Email,
                    Name = tenantUser.User.Name,
                    Locale = tenantUser.User.Locale.GetEnumMemberValue(),
                    TenantId = tenantUser.TenantId.ToString(),
                    RoleId = tenantUser.RoleId.ToString(),
                    AvatarUrl = tenantUser.User.AvatarUrl ?? GeneralConstants.Unknown,
                    IsFirstLogin = isFirstLogin,
                    // TODO: Add Role and Permission handling
                    Permissions = new List<string>()
                    // Permissions = await _dep.PlatformContext.GetUserPermissionsAsync(user.Id, tenantUser.RoleId)
                }
            },
            _dep.Localizer.GetLocalizedText(LocalizedTextKey.UserLoginSuccess),
            traceId
        );
    }
}


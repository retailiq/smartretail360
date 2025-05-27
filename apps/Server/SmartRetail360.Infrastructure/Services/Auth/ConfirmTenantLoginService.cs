using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Infrastructure.Services.Auth.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Context;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;
using SmartRetail360.Shared.Responses;
using StackExchange.Redis;

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
        _dep.UserContext.Inject(new UserExecutionContext
        {
            Action = LogActions.ConfirmTenantLogin,
            UserId = request.UserId,
            TenantId = request.TenantId,
        });
        
        var traceId = _dep.UserContext.TraceId;
        var refreshTokenExpiryDays = request.IsStaySignedIn == true
            ? _dep.AppOptions.RefreshTokenExpiryDaysWhenStaySignedIn
            : _dep.AppOptions.RefreshTokenExpiryDaysDefault;

        var tenantUserResult = await _dep.SafeExecutor.ExecuteAsync(
            () =>
                _dep.Db.TenantUsers
                    .Include(tu => tu.Tenant)
                    .Include(tu => tu.Role)
                    .Include(tu => tu.User)
                    .FirstOrDefaultAsync(tu =>
                        tu.UserId == request.UserId &&
                        tu.TenantId == request.TenantId &&
                        tu.IsActive),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );
        if (!tenantUserResult.IsSuccess)
            return tenantUserResult.ToObjectResponse().To<ConfirmTenantLoginResponse>();
        var tenantUser = tenantUserResult.Response.Data;
        var accountCheckResult = await _dep.GuardChecker
            .Check(() => tenantUser == null,
                LogEventType.ConfirmTenantLoginFailure, LogReasons.TenantUserDisabled,
                ErrorCodes.TenantUserDisabled)
            .Check(() => tenantUser is { User: null },
                LogEventType.ConfirmTenantLoginFailure, LogReasons.AccountNotFound,
                ErrorCodes.AccountNotFound)
            .Check(() => tenantUser is { Tenant: null },
                LogEventType.ConfirmTenantLoginFailure, LogReasons.TenantNotFound,
                ErrorCodes.TenantNotFound)
            .Check(() => !tenantUser!.Tenant!.IsActive,
                LogEventType.ConfirmTenantLoginFailure, LogReasons.TenantDisabled,
                ErrorCodes.TenantDisabled)
            .ValidateAsync();
        if (accountCheckResult != null)
            return accountCheckResult.To<ConfirmTenantLoginResponse>();
        
        _dep.UserContext.Inject(new UserExecutionContext
        {
            RoleId = tenantUser!.Role!.Id,
            RoleName = tenantUser.Role.Name,
            Email = tenantUser!.User!.Email,
            UserName = tenantUser.User.Name,
        });

        var isFirstLogin = tenantUser!.User!.IsFirstLogin;

        var accessToken = _dep.AccessTokenGenerator.GenerateToken(
            userId: tenantUser.Id.ToString(),
            email: tenantUser.User.Email,
            name: tenantUser.User.Name,
            tenantId: tenantUser.TenantId.ToString(),
            roleId: tenantUser.RoleId.ToString(),
            locale: tenantUser.User.Locale,
            traceId: _dep.UserContext.TraceId
        );
        
        var refreshTokenResult = await _dep.SafeExecutor.ExecuteAsync(
            () =>
                _dep.RefreshTokenService.CreateRefreshTokenAsync(
                    tenantUser.UserId,
                    tenantUser.TenantId,
                    _dep.UserContext.IpAddress,
                    refreshTokenExpiryDays
                ),
            LogEventType.ConfirmTenantLoginFailure,
            LogReasons.RefreshTokenCreationFailed,
            ErrorCodes.InternalServerError
        );
        if (!refreshTokenResult.IsSuccess)
            return refreshTokenResult.ToObjectResponse().To<ConfirmTenantLoginResponse>();
        
        var refreshToken = refreshTokenResult.Response.Data ?? Guid.NewGuid().ToString();
        
        tenantUser.User.LastLoginAt = DateTime.UtcNow;
        tenantUser.User.TraceId = traceId;
        tenantUser.User.IsFirstLogin = false;

        var saveResult = await _dep.SafeExecutor.ExecuteAsync(
            async () => { await _dep.Db.SaveChangesAsync(); },
            LogEventType.DatabaseError,
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

        await _dep.LogDispatcher.Dispatch(LogEventType.ConfirmTenantLoginSuccess);
        
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
                    Locale = tenantUser.User.Locale,
                    TenantId = tenantUser.TenantId.ToString(),
                    RoleId = tenantUser.RoleId.ToString(),
                    AvatarUrl = tenantUser.User.AvatarUrl ?? GeneralConstants.Unknown,
                    IsFirstLogin = isFirstLogin,
                    // TODO: Add Role and Permission later
                    Permissions = new List<string>()
                }
            },
            _dep.Localizer.GetLocalizedText(LocalizedTextKey.UserLoginSuccess),
            traceId
        );
    }
}
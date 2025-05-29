using Microsoft.AspNetCore.Http;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Infrastructure.Services.Auth.Login.TenantLogin;

public class ConfirmTenantLoginResponseBuilder
{
    private readonly ConfirmTenantLoginContext _ctx;

    public ConfirmTenantLoginResponseBuilder(ConfirmTenantLoginContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ApiResponse<ConfirmTenantLoginResponse>> FinalizeLoginAsync()
    {
        var user = _ctx.TenantUser!.User;

        var isFirstLogin = user!.IsFirstLogin;
        user.LastLoginAt = DateTime.UtcNow;
        user.TraceId = _ctx.TraceId;
        user.IsFirstLogin = false;

        var saveResult = await _ctx._dep.SafeExecutor.ExecuteAsync(
            async () => { await _ctx._dep.Db.SaveChangesAsync(); },
            LogEventType.DatabaseError,
            LogReasons.DatabaseSaveFailed,
            ErrorCodes.DatabaseUnavailable
        );
        if (!saveResult.IsSuccess)
            return saveResult.ToObjectResponse().To<ConfirmTenantLoginResponse>();

        _ctx._dep.HttpContext.Response.Cookies.Append(GeneralConstants.Sr360RefreshToken, _ctx.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Domain = _ctx._dep.AppOptions.CookieDomain,
            SameSite = SameSiteMode.Strict,
            Path = _ctx._dep.AppOptions.RefreshTokenPath,
            Expires = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(_ctx.RefreshTokenExpiryDays))
        });

        await _ctx._dep.LogDispatcher.Dispatch(LogEventType.ConfirmTenantLoginSuccess);

        return ApiResponse<ConfirmTenantLoginResponse>.Ok(
            new ConfirmTenantLoginResponse
            {
                AccessToken = _ctx.AccessToken,
                ExpiresIn = _ctx._dep.AppOptions.AccessTokenExpirySeconds,
                User = new AuthUserInfo
                {
                    UserId = user.Id.ToString(),
                    Email = user.Email,
                    Name = user.Name,
                    Locale = user.Locale,
                    TenantId = _ctx.TenantUser.TenantId.ToString(),
                    RoleId = _ctx.TenantUser.RoleId.ToString(),
                    AvatarUrl = user.AvatarUrl ?? GeneralConstants.Unknown,
                    IsFirstLogin = isFirstLogin,
                    Permissions = new List<string>()
                },
                Tenant = new TenantLoginCandidate
                {
                    TenantId = _ctx.TenantUser.TenantId.ToString(),
                    TenantName = _ctx.TenantUser.Tenant!.Name ?? GeneralConstants.NotSet,
                    LogoUrl = _ctx.TenantUser.Tenant.LogoUrl,
                    RoleId = _ctx.TenantUser.RoleId.ToString(),
                    RoleName = RoleHelper.ToPascalCaseName(_ctx.TenantUser.Role!.Name),
                    IsDefault = _ctx.TenantUser.IsDefault,
                    IsActive = _ctx.TenantUser.IsActive
                }
            },
            _ctx._dep.Localizer.GetLocalizedText(LocalizedTextKey.UserLoginSuccess),
            _ctx.TraceId
        );
    }
}

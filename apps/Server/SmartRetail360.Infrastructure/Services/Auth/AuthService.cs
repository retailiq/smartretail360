using Microsoft.AspNetCore.Http;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Infrastructure.Services.Auth.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AuthTokenDependencies _dep;

    public AuthService(
        AuthTokenDependencies dep)
    {
        _dep = dep;
    }

    public async Task<ApiResponse<RefreshTokenResponse>> RefreshAsync(RefreshTokenRequest request)
    {
        var oldRefreshToken = _dep.HttpContext.Request.Cookies[GeneralConstants.Sr360RefreshToken];

        var refreshTokenExpiryDays = request.IsStaySignedIn == true
            ? _dep.AppOptions.RefreshTokenExpiryDaysWhenStaySignedIn
            : _dep.AppOptions.RefreshTokenExpiryDaysDefault;

        var (newToken, tokenEntity) = await _dep.RefreshTokenService.RotateRefreshTokenAsync(
            oldToken: oldRefreshToken ?? string.Empty,
            ipAddress: _dep.UserContext.IpAddress,
            expiryDays: refreshTokenExpiryDays
        );

        var refreshTokenCheckResult = await _dep.GuardChecker
            .Check(() => newToken == null || tokenEntity == null,
                LogEventType.RefreshTokenFailure, LogReasons.RefreshTokenNotFound,
                ErrorCodes.RefreshTokenNotFound)
            .ValidateAsync();
        if (refreshTokenCheckResult != null)
        {
            _dep.HttpContext.Response.Cookies.Delete(GeneralConstants.Sr360RefreshToken);
            return refreshTokenCheckResult.To<RefreshTokenResponse>();
        }

        var newAccessToken = _dep.AccessTokenGenerator.GenerateToken(
            userId: tokenEntity!.UserId.ToString(),
            email: tokenEntity.Email,
            name: tokenEntity.Name,
            tenantId: tokenEntity.TenantId.ToString(),
            roleId: tokenEntity.RoleId.ToString(),
            locale: tokenEntity.Locale,
            traceId: _dep.UserContext.TraceId
        );

        _dep.HttpContext.Response.Cookies.Append(GeneralConstants.Sr360RefreshToken, newToken!,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Domain = _dep.AppOptions.CookieDomain,
                SameSite = SameSiteMode.Strict,
                Path = _dep.AppOptions.RefreshTokenPath,
                Expires = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(refreshTokenExpiryDays))
            });

        return ApiResponse<RefreshTokenResponse>.Ok(
            new RefreshTokenResponse
            {
                AccessToken = newAccessToken
            },
            _dep.Localizer.GetLocalizedText(LocalizedTextKey.RefreshTokenSuccess),
            _dep.UserContext.TraceId
        );
    }

    public async Task<ApiResponse<object>> LogoutAsync()
    {
        var oldRefreshToken = _dep.HttpContext.Request.Cookies[GeneralConstants.Sr360RefreshToken];

        await _dep.RefreshTokenService.RevokeRefreshTokenAsync(
            token: oldRefreshToken ?? string.Empty,
            ipAddress: _dep.UserContext.IpAddress,
            reason: RefreshTokenRevokeReason.Logout);

        _dep.HttpContext.Response.Cookies.Delete(GeneralConstants.Sr360RefreshToken);

        return ApiResponse<object>.Ok(
            null,
            _dep.Localizer.GetLocalizedText(LocalizedTextKey.LogoutSuccess),
            _dep.UserContext.TraceId
        );
    }
}
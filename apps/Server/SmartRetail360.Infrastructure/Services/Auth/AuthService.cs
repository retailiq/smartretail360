using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Infrastructure.Services.Auth.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;
using SmartRetail360.Shared.Utils;
using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.Models;
using SmartRetail360.Auth.Validators;

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
        _dep.UserContext.Inject(new UserExecutionContext { Action = LogActions.RefreshToken });

        var oldRefreshToken = _dep.HttpContext.Request.Cookies[GeneralConstants.Sr360RefreshToken];
        var oldHash = TokenHelper.HashToken(oldRefreshToken ?? string.Empty);
        var tokenEntityResult = await _dep.SafeExecutor.ExecuteAsync(
            () =>
                _dep.Db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == oldHash),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );
        var tokenEntity = tokenEntityResult.Response.Data;
        var validityCheckResult = await _dep.GuardChecker
            .Check(() => tokenEntity == null,
                LogEventType.RefreshTokenFailure, LogReasons.RefreshTokenMissing,
                ErrorCodes.RefreshTokenMissing)
            .Check(() => tokenEntity!.IsExpired,
                LogEventType.RefreshTokenFailure, LogReasons.RefreshTokenExpired,
                ErrorCodes.RefreshTokenExpired)
            .Check(() => tokenEntity!.IsRevoked,
                LogEventType.RefreshTokenFailure, LogReasons.RefreshTokenRevoked,
                ErrorCodes.RefreshTokenRevoked)
            .Check(() => tokenEntity!.ReplacedByToken != null,
                LogEventType.RefreshTokenReplayDetected, LogReasons.RefreshTokenReplayDetected,
                ErrorCodes.RefreshTokenReplayDetected)
            .ValidateAsync();
        if (validityCheckResult != null)
        {
            _dep.HttpContext.Response.Cookies.Delete(GeneralConstants.Sr360RefreshToken);
            return validityCheckResult.To<RefreshTokenResponse>();
        }

        _dep.UserContext.Inject(new UserExecutionContext
        {
            Email = tokenEntity!.Email,
            UserName = tokenEntity.UserName,
            UserId = tokenEntity.UserId,
            TenantId = tokenEntity.TenantId,
            RoleId = tokenEntity.RoleId
        });

        var newToken = await _dep.RefreshTokenService.RotateRefreshTokenAsync(
            oldEntity: tokenEntity,
            ipAddress: _dep.UserContext.IpAddress
        );

        var refreshTokenCheckResult = await _dep.GuardChecker
            .Check(() => newToken == null,
                LogEventType.DatabaseError, LogReasons.DatabaseSaveFailed,
                ErrorCodes.DatabaseUnavailable)
            .ValidateAsync();
        if (refreshTokenCheckResult != null)
        {
            _dep.HttpContext.Response.Cookies.Delete(GeneralConstants.Sr360RefreshToken);
            return refreshTokenCheckResult.To<RefreshTokenResponse>();
        }

        var newAccessToken = _dep.AccessTokenGenerator.GenerateToken(new AccessTokenCreationContext
        {
            UserId = tokenEntity.UserId.ToString(),
            Email = tokenEntity.Email,
            UserName = tokenEntity.UserName,
            TenantId = tokenEntity.TenantId.ToString(),
            RoleId = tokenEntity.RoleId.ToString(),
            RoleName = tokenEntity.RoleName,
            TraceId = _dep.UserContext.TraceId,
            Environment = tokenEntity.Env
        });

        _dep.HttpContext.Response.Cookies.Append(GeneralConstants.Sr360RefreshToken, newToken!,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Domain = _dep.AppOptions.CookieDomain,
                SameSite = SameSiteMode.Strict,
                Path = _dep.AppOptions.RefreshTokenPath,
                Expires = tokenEntity.ExpiresAt
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
        _dep.UserContext.Inject(new UserExecutionContext { Action = LogActions.Logout });
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

    public async Task<ApiResponse<object>> ValidateToken()
    {
        _dep.UserContext.Inject(new UserExecutionContext { Action = LogActions.ValidateToken });

        var token = _dep.HttpContext.Request.Headers["Authorization"]
            .FirstOrDefault()?.Replace("Bearer ", "");

        var tokenCheckResult = await _dep.GuardChecker
            .Check(() => string.IsNullOrWhiteSpace(token),
                LogEventType.TokenValidationFailure, LogReasons.TokenMissing,
                ErrorCodes.TokenMissing)
            .ValidateAsync();
        if (tokenCheckResult != null)
            return tokenCheckResult.To<object>();

        var valid = JwtTokenValidator.TryValidateToken(
            token!,
            _dep.AppOptions.JwtSecret,
            out _,
            out _,
            out var ex
        );

        var expiredCheck = await _dep.GuardChecker
            .Check(() => !valid && ex is SecurityTokenExpiredException,
                LogEventType.TokenValidationFailure,
                LogReasons.TokenExpired,
                ErrorCodes.TokenExpired)
            .ValidateAsync();

        if (expiredCheck != null)
            return expiredCheck.To<object>();

        var invalidCheck = await _dep.GuardChecker
            .Check(() => !valid && ex is not SecurityTokenExpiredException,
                LogEventType.TokenValidationFailure,
                LogReasons.InvalidToken,
                ErrorCodes.InvalidToken)
            .ValidateAsync();

        if (invalidCheck != null)
            return invalidCheck.To<object>();

        return ApiResponse<object>.Ok(
            null,
            _dep.Localizer.GetLocalizedText(LocalizedTextKey.TokenValid),
            _dep.UserContext.TraceId
        );
    }
}
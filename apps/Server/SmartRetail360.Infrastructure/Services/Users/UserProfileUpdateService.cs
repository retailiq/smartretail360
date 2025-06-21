using Microsoft.AspNetCore.Http;
using SmartRetail360.Application.Interfaces.Users;
using SmartRetail360.Contracts.Users.Requests;
using SmartRetail360.Contracts.Users.Responses;
using SmartRetail360.Infrastructure.Services.Users.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Users;

public partial class UserProfileUpdateService : IUserProfileUpdateService
{
    private readonly UsersDependencies _dep;

    public UserProfileUpdateService(
        UsersDependencies usersDependencies)
    {
        _dep = usersDependencies;
    }

    public async Task<ApiResponse<object>> UpdateUserPassword(UpdateUserPasswordRequest request, Guid userId)
    {
        return null;
    }

    public async Task<ApiResponse<UpdateUserEmailResponse>> UpdateUserEmail(UpdateUserEmailRequest request, Guid userId)
    {
        return null;
    }

    private async Task<ApiResponse<T>?> ValidateAndSetRefreshTokenCookieAsync<T>(
        string? refreshToken,
        DateTime? expiresAt)
    {
        var checkResult = await _dep.GuardChecker
            .Check(() => refreshToken == null,
                LogEventType.DatabaseError,
                LogReasons.DatabaseSaveFailed,
                ErrorCodes.DatabaseUnavailable)
            .ValidateAsync();

        if (checkResult != null)
        {
            _dep.HttpContext.Response.Cookies.Delete(GeneralConstants.Sr360RefreshToken);
            return checkResult.To<T>();
        }

        _dep.HttpContext.Response.Cookies.Append(GeneralConstants.Sr360RefreshToken, refreshToken!,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Domain = _dep.AppOptions.CookieDomain,
                SameSite = SameSiteMode.Strict,
                Path = _dep.AppOptions.RefreshTokenPath,
                Expires = expiresAt
            });

        return null!;
    }
}
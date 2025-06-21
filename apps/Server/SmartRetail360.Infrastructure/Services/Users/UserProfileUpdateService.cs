using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SmartRetail360.Application.Interfaces.Users;
using SmartRetail360.Contracts.Users.Requests;
using SmartRetail360.Contracts.Users.Responses;
using SmartRetail360.Execution;
using SmartRetail360.Platform.Interfaces;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Options;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Users;

public partial class UserProfileUpdateService : IUserProfileUpdateService
{
    private readonly IPlatformContextService _platformContextService;
    private readonly IGuardChecker _guardChecker;
    private readonly MessageLocalizer _localizer;
    private readonly IUserContextService _userContext;
    private readonly UpdateUserProfileTokenGenerator _tokenGenerator;
    private readonly IHttpContextAccessor _httpContext;
    private readonly AppOptions _appOptions;

    public UserProfileUpdateService(
        IPlatformContextService platformContextService,
        IGuardChecker guardChecker,
        MessageLocalizer localizer,
        IUserContextService userContextService,
        UpdateUserProfileTokenGenerator tokenGenerator,
        IHttpContextAccessor httpContext,
        IOptions<AppOptions> appOptions)
    {
        _platformContextService = platformContextService;
        _guardChecker = guardChecker;
        _localizer = localizer;
        _userContext = userContextService;
        _tokenGenerator = tokenGenerator;
        _httpContext = httpContext;
        _appOptions = appOptions.Value;
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
        var checkResult = await _guardChecker
            .Check(() => refreshToken == null,
                LogEventType.DatabaseError,
                LogReasons.DatabaseSaveFailed,
                ErrorCodes.DatabaseUnavailable)
            .ValidateAsync();

        if (checkResult != null)
        {
            _httpContext.HttpContext?.Response.Cookies.Delete(GeneralConstants.Sr360RefreshToken);
            return checkResult.To<T>();
        }

        _httpContext.HttpContext?.Response.Cookies.Append(GeneralConstants.Sr360RefreshToken, refreshToken!,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Domain = _appOptions.CookieDomain,
                SameSite = SameSiteMode.Strict,
                Path = _appOptions.RefreshTokenPath,
                Expires = expiresAt
            });

        return null!;
    }
}
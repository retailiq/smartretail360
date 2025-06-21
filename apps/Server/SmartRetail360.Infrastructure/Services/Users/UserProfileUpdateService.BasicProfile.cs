using SmartRetail360.Contracts.Users.Requests;
using SmartRetail360.Contracts.Users.Responses;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Users;

public partial class UserProfileUpdateService
{
    public async Task<ApiResponse<UpdateUserBasicProfileResponse>> UpdateUserBasicProfile(
        UpdateUserBasicProfileRequest request, Guid userId)
    {
        var (existingTenantUser, error) =
            await _platformContextService.GetTenantUserByTenantAndUserIdAsync(userId,
                _userContext.TenantId ?? Guid.Empty);
        if (error != null)
            return error.To<UpdateUserBasicProfileResponse>();

        var existingUserCheckResult = await _guardChecker
            .Check(() => existingTenantUser == null, LogEventType.UpdateUserBasicProfileFailure,
                LogReasons.TenantUserRecordNotFound,
                ErrorCodes.TenantUserRecordNotFound)
            .Check(() => existingTenantUser!.User == null, LogEventType.UpdateUserBasicProfileFailure,
                LogReasons.UserNotExists,
                ErrorCodes.UserNotExists)
            .ValidateAsync();
        if (existingUserCheckResult != null)
            return existingUserCheckResult.To<UpdateUserBasicProfileResponse>();

        var hasChanges = false;
        var response = new UpdateUserBasicProfileResponse();

        if (request.Name is { } name && existingTenantUser!.User!.Name != name)
        {
            existingTenantUser.User.Name = name;
            response.Name = name;
            hasChanges = true;
        }

        if (request.CountryCode is { } code && existingTenantUser!.User!.CountryCode != code)
        {
            existingTenantUser.User.CountryCode = code;
            response.CountryCode = code;
            hasChanges = true;
        }

        if (request.PhoneNumber is { } phone && existingTenantUser!.User!.PhoneNumber != phone)
        {
            existingTenantUser.User.PhoneNumber = phone;
            response.PhoneNumber = phone;
            hasChanges = true;
        }

        if (request.AvatarUrl is { } avatarUrl && existingTenantUser!.User!.AvatarUrl != avatarUrl)
        {
            existingTenantUser.User.AvatarUrl = avatarUrl;
            response.AvatarUrl = avatarUrl;
            hasChanges = true;
        }

        if (request.Locale is { } locale && existingTenantUser!.User!.LocaleEnum != locale)
        {
            existingTenantUser.User.LocaleEnum = locale;
            response.Locale = existingTenantUser.User.Locale;
            hasChanges = true;
        }

        if (!hasChanges)
        {
            return ApiResponse<UpdateUserBasicProfileResponse>.Ok(
                null,
                _localizer.GetLocalizedText(LocalizedTextKey.UserProfileUnchanged),
                _userContext.TraceId
            );
        }

        var saveResult = await _platformContextService.SaveChangesAsync();
        if (saveResult != null)
            return saveResult.To<UpdateUserBasicProfileResponse>();

        var oldRefreshToken = _httpContext.HttpContext?.Request.Cookies[GeneralConstants.Sr360RefreshToken];

        var tokens = await _tokenGenerator.GenerateTokensAsync(
            existingTenantUser!,
            _userContext.TraceId,
            _userContext.Env.GetEnumMemberValue(),
            _userContext.IpAddress,
            oldRefreshToken ?? string.Empty
        );

        response.AccessToken = tokens.AccessToken;

        var refreshTokenResult = await ValidateAndSetRefreshTokenCookieAsync<UpdateUserBasicProfileResponse>(
            tokens.RefreshToken,
            tokens.ExpiresAt
        );
        if (refreshTokenResult != null)
            return refreshTokenResult;

        return ApiResponse<UpdateUserBasicProfileResponse>.Ok(
            response,
            _localizer.GetLocalizedText(LocalizedTextKey.UpdateUserBasicProfileSuccessfully),
            _userContext.TraceId
        );
    }
}
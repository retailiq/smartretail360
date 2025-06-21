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
            await _dep.PlatformContext.GetTenantUserByTenantAndUserIdAsync(userId,
                _dep.UserContext.TenantId ?? Guid.Empty);
        if (error != null)
            return error.To<UpdateUserBasicProfileResponse>();

        var existingUserCheckResult = await _dep.GuardChecker
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
            await _dep.LogDispatcher.Dispatch(LogEventType.UpdateUserBasicProfileFailure,
                LogReasons.BasicProfileUnchanged);
            return ApiResponse<UpdateUserBasicProfileResponse>.Ok(
                null,
                _dep.Localizer.GetLocalizedText(LocalizedTextKey.UserProfileUnchanged),
                _dep.UserContext.TraceId
            );
        }

        existingTenantUser!.User!.TraceId = _dep.UserContext.TraceId;
        existingTenantUser.User.LastUpdatedBy = _dep.UserContext.UserId;

        var saveResult = await _dep.PlatformContext.SaveChangesAsync();
        if (saveResult != null)
            return saveResult.To<UpdateUserBasicProfileResponse>();

        var oldRefreshToken = _dep.HttpContext.Request.Cookies[GeneralConstants.Sr360RefreshToken];

        var tokens = await _dep.UpdateUserProfileTokenGenerator.GenerateTokensAsync(
            existingTenantUser,
            _dep.UserContext.TraceId,
            _dep.UserContext.Env.GetEnumMemberValue(),
            _dep.UserContext.IpAddress,
            oldRefreshToken ?? string.Empty
        );

        response.AccessToken = tokens.AccessToken;

        var refreshTokenResult = await ValidateAndSetRefreshTokenCookieAsync<UpdateUserBasicProfileResponse>(
            tokens.RefreshToken,
            tokens.ExpiresAt
        );
        if (refreshTokenResult != null)
            return refreshTokenResult;

        await _dep.LogDispatcher.Dispatch(LogEventType.UpdateUserBasicProfileSuccess);

        return ApiResponse<UpdateUserBasicProfileResponse>.Ok(
            response,
            _dep.Localizer.GetLocalizedText(LocalizedTextKey.UpdateUserBasicProfileSuccessfully),
            _dep.UserContext.TraceId
        );
    }
}
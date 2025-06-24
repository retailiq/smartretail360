using SmartRetail360.Contracts.Users.Requests;
using SmartRetail360.Contracts.Users.Responses;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;
using SmartRetail360.Shared.Messaging.Factories;
using SmartRetail360.Shared.Responses;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Infrastructure.Services.Users;

public partial class UserProfileUpdateService
{
    public async Task<ApiResponse<UpdateUserEmailResponse>> UpdateUserEmail(UpdateUserEmailRequest request, Guid userId)
    {
        var (existingTenantUser, error) =
            await _dep.PlatformContext.GetTenantUserByTenantAndUserIdAsync(userId,
                _dep.UserContext.TenantId ?? Guid.Empty);
        if (error != null)
            return error.To<UpdateUserEmailResponse>();
        var (existingUser, userError) = await _dep.PlatformContext.GetUserByEmailAsync(request.NewEmail);
        if (userError != null)
            return userError.To<UpdateUserEmailResponse>();

        var existingUserCheckResult = await _dep.GuardChecker
            .Check(() => existingTenantUser == null, LogEventType.UpdateUserEmailFailure,
                LogReasons.TenantUserRecordNotFound,
                ErrorCodes.TenantUserRecordNotFound)
            .Check(() => existingTenantUser!.User == null, LogEventType.UpdateUserEmailFailure,
                LogReasons.UserNotExists,
                ErrorCodes.UserNotExists)
            .Check(() => existingUser != null && existingUser.Email != existingTenantUser!.User!.Email,
                LogEventType.UpdateUserEmailFailure,
                LogReasons.EmailAlreadyUsed,
                ErrorCodes.EmailAlreadyUsed)
            .ValidateAsync();
        if (existingUserCheckResult != null)
            return existingUserCheckResult.To<UpdateUserEmailResponse>();

        if (request.NewEmail == existingTenantUser!.User!.Email)
        {
            await _dep.LogDispatcher.Dispatch(LogEventType.UpdateUserEmailFailure,
                LogReasons.UserEmailUnchanged);
            return ApiResponse<UpdateUserEmailResponse>.Ok(
                null,
                _dep.Localizer.GetLocalizedText(LocalizedTextKey.UserEmailUnchanged),
                _dep.UserContext.TraceId
            );
        }

        existingTenantUser.User.Email = request.NewEmail;
        existingTenantUser.User!.TraceId = _dep.UserContext.TraceId;
        existingTenantUser.User.LastUpdatedBy = _dep.UserContext.UserId;

        var emailVerificationToken = TokenHelper.GenerateActivateAccountToken();
        var accountToken = new AccountToken
        {
            UserId = existingTenantUser.UserId,
            TenantId = existingTenantUser.TenantId,
            Token = emailVerificationToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_dep.AppOptions.EmailValidityPeriodMinutes),
            TraceId = _dep.UserContext.TraceId,
            Email = existingTenantUser.User.Email
        };

        var saveResult = await _dep.SafeExecutor.ExecuteAsync(
            async () =>
            {
                _dep.Db.AccountTokens.Add(accountToken);
                await _dep.Db.SaveChangesAsync();
                await _dep.RedisOperation.SetActivationTokenAsync(accountToken);
            },
            LogEventType.DatabaseError,
            LogReasons.DatabaseSaveFailed,
            ErrorCodes.DatabaseUnavailable
        );

        if (!saveResult.IsSuccess)
            return saveResult.ToObjectResponse().To<UpdateUserEmailResponse>();

        var payload = EmailSendingPayloadFactory.Create(
            existingTenantUser.User.Email,
            existingTenantUser.User.Name,
            emailVerificationToken,
            LogActions.SendUserEmailUpdateVerificationEmail,
            EmailTemplate.UseEmailUpdate,
            _dep.AppOptions.EmailValidityPeriodMinutes
        );

        var emailError =
            await _dep.PlatformContext.SendEmailSqsMessageAsync(emailVerificationToken, payload);
        if (emailError != null)
            return emailError.To<UpdateUserEmailResponse>();

        var oldRefreshToken = _dep.HttpContext.Request.Cookies[GeneralConstants.Sr360RefreshToken];

        var tokens = await _dep.UpdateUserProfileTokenGenerator.GenerateTokensAsync(
            existingTenantUser,
            _dep.UserContext.TraceId,
            _dep.UserContext.Env.GetEnumMemberValue(),
            _dep.UserContext.IpAddress,
            oldRefreshToken ?? string.Empty
        );

        var refreshTokenResult = await ValidateAndSetRefreshTokenCookieAsync<UpdateUserEmailResponse>(
            tokens.RefreshToken,
            tokens.ExpiresAt
        );
        if (refreshTokenResult != null)
            return refreshTokenResult;

        await _dep.LogDispatcher.Dispatch(LogEventType.UpdateUserEmailSuccess);

        return ApiResponse<UpdateUserEmailResponse>.Ok(
            // new UpdateUserEmailResponse
            // {
            //     // AccessToken = tokens.AccessToken,
            //     Email = request.NewEmail
            // },
            null,
            _dep.Localizer.GetLocalizedText(LocalizedTextKey.UserEmailUpdateVerificationLinkSent,
                _dep.AppOptions.EmailValidityPeriodMinutes),
            _dep.UserContext.TraceId
        );
    }
}
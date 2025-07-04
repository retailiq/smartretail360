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

        existingTenantUser.User.NewEmail = request.NewEmail;
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
            Email = existingTenantUser.User.Email,
            NewEmail = request.NewEmail,
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
            email: existingTenantUser.User.Email,
            userName: existingTenantUser.User.Name,
            token: emailVerificationToken,
            action: LogActions.SendUserEmailUpdateVerificationEmail,
            template: EmailTemplate.EmailUpdate,
            minutes: _dep.AppOptions.EmailValidityPeriodMinutes,
            newEmail: request.NewEmail
        );

        var emailError =
            await _dep.PlatformContext.SendEmailSqsMessageAsync(emailVerificationToken, payload);
        if (emailError != null)
            return emailError.To<UpdateUserEmailResponse>();

        await _dep.LogDispatcher.Dispatch(LogEventType.RegisterUserSuccess);

        return ApiResponse<UpdateUserEmailResponse>.Ok(
            new UpdateUserEmailResponse
            {
                NewEmail = request.NewEmail
            },
            _dep.Localizer.GetLocalizedText(LocalizedTextKey.UserEmailUpdateVerificationLinkSent,
                _dep.AppOptions.EmailValidityPeriodMinutes),
            _dep.UserContext.TraceId
        );
    }
}
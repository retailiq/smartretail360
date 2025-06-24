using SmartRetail360.Application.Interfaces.Notifications;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Services.Notifications.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Messaging.Factories;
using SmartRetail360.Shared.Responses;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Infrastructure.Services.Notifications;

public class AccountActivationEmailResendingService : IAccountActivationEmailResendingService
{
    private readonly NotificationDependencies _dep;

    public AccountActivationEmailResendingService(NotificationDependencies dep)
    {
        _dep = dep;
    }

    public async Task<ApiResponse<object>> ResendEmailAsync(string email)
    {
        _dep.UserContext.Inject(new UserExecutionContext { Email = email });

        var (existingUser, userError) = await _dep.PlatformContext.GetUserByEmailAsync(email);
        if (userError != null)
            return userError;

        var userCheckResult = await _dep.GuardChecker
            .Check(() => existingUser == null,
                LogEventType.EmailSendFailure, LogReasons.AccountNotFound,
                ErrorCodes.AccountNotFound)
            .ValidateAsync();
        if (userCheckResult != null)
            return userCheckResult;

        var (tokenList, tokenListError) = await _dep.AccountSupport.GetActivationTokenListAsync(existingUser!.Id);
        if (tokenListError != null)
            return tokenListError;
        var tokenListCheckResult = await _dep.GuardChecker
            .Check(() => tokenList!.Count == 0,
                LogEventType.EmailSendFailure, LogReasons.TokenNotFound,
                ErrorCodes.TokenNotFound)
            .ValidateAsync();
        if (tokenListCheckResult != null)
            return tokenListCheckResult;

        var latestToken = tokenList!.FirstOrDefault();
        var latestSource = latestToken?.SourceEnum ?? ActivationSource.None;
        var action = TokenHelper.GetLogAction(latestSource);
        _dep.UserContext.Inject(new UserExecutionContext
        {
            UserId = existingUser.Id,
            Action = action
        });

        var isLimited = await _dep.RedisOperation.IsEmailResendLimitedAsync(email);

        var guardResult = await _dep.GuardChecker
            .Check(() => existingUser is { IsEmailVerified: true, StatusEnum: AccountStatus.Active },
                LogEventType.EmailSendFailure, LogReasons.AccountAlreadyActivated, ErrorCodes.AccountAlreadyActivated)
            .Check(() => isLimited, LogEventType.EmailSendFailure, LogReasons.TooFrequentEmailRequest,
                ErrorCodes.TooFrequentEmailRequest)
            .ValidateAsync();

        if (guardResult != null)
            return guardResult;

        var pendingTokens = tokenList!
            .Where(t => t.StatusEnum == ActivationTokenStatus.Pending)
            .OrderByDescending(t => t.CreatedAt)
            .ToList();

        var latestPendingToken = pendingTokens.FirstOrDefault();
        var fallbackToken = tokenList!
            .Where(t => t.StatusEnum is ActivationTokenStatus.Expired or ActivationTokenStatus.Revoked)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefault();

        var tenantIdToUse = latestPendingToken?.TenantId ?? fallbackToken?.TenantId;

        var checkResult = await _dep.GuardChecker
            .Check(() => pendingTokens.Count > 0 && latestPendingToken?.ExpiresAt > DateTime.UtcNow,
                LogEventType.EmailSendFailure,
                LogReasons.HasPendingActivationEmail,
                ErrorCodes.HasPendingActivationEmail)
            .Check(() => tenantIdToUse == null,
                LogEventType.EmailSendFailure,
                LogReasons.TokenNotFound,
                ErrorCodes.TokenNotFound)
            .ValidateAsync();

        if (checkResult != null)
            return checkResult;

        if (latestPendingToken != null)
        {
            latestPendingToken.StatusEnum = ActivationTokenStatus.Revoked;
            latestPendingToken.TraceId = _dep.UserContext.TraceId;
        }

        var emailVerificationToken = TokenHelper.GenerateActivateAccountToken();
        var accountActivationToken = new AccountToken
        {
            UserId = existingUser.Id,
            TenantId = tenantIdToUse!.Value,
            Token = emailVerificationToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_dep.AppOptions.EmailValidityPeriodMinutes),
            TraceId = _dep.UserContext.TraceId,
            SourceEnum = latestSource
        };
        existingUser.LastEmailSentAt = DateTime.UtcNow;

        var saveResult = await _dep.SafeExecutor.ExecuteAsync(
            async () =>
            {
                _dep.Db.AccountTokens.Add(accountActivationToken);
                await _dep.Db.SaveChangesAsync();
                await _dep.RedisOperation.SetActivationTokenAsync(accountActivationToken);
            },
            LogEventType.DatabaseError,
            LogReasons.DatabaseSaveFailed,
            ErrorCodes.DatabaseUnavailable
        );

        if (!saveResult.IsSuccess)
            return saveResult.ToObjectResponse();

        EmailTemplate emailTemplate = latestSource == ActivationSource.Registration
            ? EmailTemplate.UserRegistrationActivation
            : EmailTemplate.UserInvitationActivation;

        var payload = EmailSendingPayloadFactory.Create(
            existingUser.Email,
            existingUser.Name,
            emailVerificationToken,
            action,
            emailTemplate,
            _dep.AppOptions.EmailValidityPeriodMinutes
        );

        var emailError =
            await _dep.PlatformContext.SendEmailSqsMessageAsync(emailVerificationToken, payload);
        if (emailError != null)
            return emailError;

        await _dep.RedisOperation.SetEmailResendLimitAsync(email);
        await _dep.LogDispatcher.Dispatch(LogEventType.EmailSendSuccess);

        return ApiResponse<object>.Ok(
            null,
            _dep.Localizer.GetLocalizedText(LocalizedTextKey.EmailResent,
                _dep.AppOptions.EmailValidityPeriodMinutes),
            traceId: _dep.UserContext.TraceId
        );
    }
}
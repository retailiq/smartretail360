using SmartRetail360.Application.Interfaces.Notifications;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;
using SmartRetail360.Shared.Utils;
using SmartRetail360.Infrastructure.Services.Notifications.Models;
using SmartRetail360.Shared.Context;
using SmartRetail360.Shared.Messaging.Factories;
using SmartRetail360.Shared.Redis;

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

        var redisKey = RedisKeys.ResendAccountActivationEmail(email);
        var isLimited = await _dep.RedisLimiterService.IsLimitedAsync(redisKey);

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

        var checkResult = await _dep.GuardChecker
            .Check(() => pendingTokens.Count > 0 && latestPendingToken?.ExpiresAt > DateTime.UtcNow,
                LogEventType.AccountActivateFailure,
                LogReasons.HasPendingActivationEmail,
                ErrorCodes.HasPendingActivationEmail)
            .ValidateAsync();

        if (checkResult != null)
            return checkResult;

        if (latestPendingToken != null)
        {
            latestPendingToken.StatusEnum = ActivationTokenStatus.Revoked;
            latestPendingToken.TraceId = _dep.UserContext.TraceId;
        }

        var emailVerificationToken = TokenHelper.GenerateActivateAccountToken();
        var accountActivationToken = new AccountActivationToken
        {
            UserId = existingUser.Id,
            TenantId = latestPendingToken!.TenantId,
            Token = emailVerificationToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_dep.AppOptions.ActivationTokenLimitMinutes),
            TraceId = _dep.UserContext.TraceId,
            SourceEnum = latestSource
        };
        existingUser.LastEmailSentAt = DateTime.UtcNow;

        var saveResult = await _dep.SafeExecutor.ExecuteAsync(
            async () =>
            {
                _dep.Db.AccountActivationTokens.Add(accountActivationToken);
                await _dep.Db.SaveChangesAsync();
                await _dep.RedisOperation.SetActivationTokenAsync(accountActivationToken,
                    TimeSpan.FromMinutes(_dep.AppOptions.ActivationTokenLimitMinutes));
            },
            LogEventType.EmailSendFailure,
            LogReasons.DatabaseSaveFailed,
            ErrorCodes.DatabaseUnavailable
        );

        if (!saveResult.IsSuccess)
            return saveResult.ToObjectResponse();

        EmailTemplate emailTemplate = latestSource == ActivationSource.Registration
            ? EmailTemplate.UserRegistrationActivation
            : EmailTemplate.UserInvitationActivation;

        var payload = ActivationEmailPayloadFactory.Create(
            existingUser.Email,
            existingUser.Name,
            emailVerificationToken,
            action,
            emailTemplate,
            _dep.AppOptions.ActivationTokenLimitMinutes
        );

        var emailError =
            await _dep.PlatformContext.SendRegistrationInvitationEmailAsync(emailVerificationToken, payload);
        if (emailError != null)
            return emailError;

        await _dep.RedisOperation.SetLimitAsync(redisKey,
            TimeSpan.FromMinutes(_dep.AppOptions.EmailSendLimitMinutes));
        await _dep.LogDispatcher.Dispatch(LogEventType.EmailSendSuccess);

        return ApiResponse<object>.Ok(
            null,
            _dep.Localizer.GetLocalizedText(LocalizedTextKey.EmailResent,
                _dep.AppOptions.AccountActivationLimitMinutes),
            traceId: _dep.UserContext.TraceId
        );
    }
}
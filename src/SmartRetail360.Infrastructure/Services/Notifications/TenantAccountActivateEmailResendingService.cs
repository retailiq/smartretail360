using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.Interfaces.Notifications;
using SmartRetail360.Infrastructure.DTOs.Messaging;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;
using SmartRetail360.Shared.Utils;
using SmartRetail360.Infrastructure.Services.Notifications.Models;
using SmartRetail360.Shared.Redis;

namespace SmartRetail360.Infrastructure.Services.Notifications;

public class TenantAccountActivateEmailResendingService : IAccountActivateEmailResendingService
{
    private readonly NotificationDependencies _dep;

    public TenantAccountActivateEmailResendingService(NotificationDependencies dep)
    {
        _dep = dep;
    }

    public async Task<ApiResponse<object>> ResendEmailAsync(string email)
    {
        var tenantResult = await _dep.SafeExecutor.ExecuteAsync(
            () => _dep.Db.Tenants.FirstOrDefaultAsync(t => t.AdminEmail == email),
            LogEventType.EmailSendFailure,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable,
            email: email
        );

        if (!tenantResult.IsSuccess)
            return tenantResult.ToObjectResponse();

        var tenant = tenantResult.Response.Data;
        var redisKey = RedisKeys.ResendAccountActivationEmail(email);
        var isLimited = await _dep.RedisLimiterService.IsLimitedAsync(redisKey);
        
        var guard = new GuardChecker(_dep.LogDispatcher, _dep.UserContext, _dep.Localizer)
            .WithEmail(email)
            .WithTenantId(tenant?.Id);

        var failed = await guard
            .Check(() => tenant == null, LogEventType.EmailSendFailure, LogReasons.TenantNotFound, ErrorCodes.TenantNotFound)
            .Check(() => tenant!.IsEmailVerified, LogEventType.EmailSendFailure, LogReasons.TenantAccountAlreadyActivated, ErrorCodes.AccountAlreadyActivated)
            .CheckAsync(() => Task.FromResult(isLimited), LogEventType.EmailSendFailure, LogReasons.TooFrequentEmailRequest, ErrorCodes.TooFrequentEmailRequest)
            .ValidateAsync();

        if (failed != null)
            return failed;

        tenant.EmailVerificationToken = TokenGenerator.GenerateActivateAccountToken();
        tenant.LastEmailSentAt = DateTime.UtcNow;

        var saveResult = await _dep.SafeExecutor.ExecuteAsync(
            async () => { await _dep.Db.SaveChangesAsync(); },
            LogEventType.EmailSendFailure,
            LogReasons.DatabaseSaveFailed,
            ErrorCodes.DatabaseUnavailable,
            email: email,
            tenantId: tenant.Id
        );

        if (!saveResult.IsSuccess)
            return saveResult.ToObjectResponse();

        var payload = new ActivationEmailPayload
        {
            Email = tenant.AdminEmail,
            Token = tenant.EmailVerificationToken,
            TraceId = _dep.UserContext.TraceId ?? Guid.NewGuid().ToString("N"),
            TenantId = tenant.Id,
            Locale = _dep.UserContext.Locale ?? "en",
            Timestamp = DateTime.UtcNow.ToString("o")
        };

        var sendResult = await _dep.SafeExecutor.ExecuteAsync(
            async () => { await _dep.EmailQueueProducer.SendAsync(payload); },
            LogEventType.EmailSendFailure,
            LogReasons.SendSqsMessageFailed,
            ErrorCodes.EmailSendFailed
        );

        if (!sendResult.IsSuccess)
            return sendResult.ToObjectResponse();

        await _dep.RedisLimiterService.SetLimitAsync(redisKey, TimeSpan.FromMinutes(_dep.AppOptions.EmailSendLimitMinutes));
        await _dep.LogDispatcher.Dispatch(LogEventType.EmailSendSuccess, email: email, tenantId: tenant.Id);

        return ApiResponse<object>.Ok(
            null,
            _dep.Localizer.GetSuccessMessage(SuccessCodes.EmailResent),
            traceId: _dep.UserContext.TraceId
        );
    }
}

using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.Extensions;
using SmartRetail360.Application.Interfaces.Notifications;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;
using SmartRetail360.Shared.Utils;
using SmartRetail360.Infrastructure.Services.Notifications.Models;
using SmartRetail360.Shared.DTOs.Messaging;
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
        _dep.UserContext.Inject(clientEmail: email, action: LogActions.TenantAccountActivateEmailReSend);
        
        var tenantResult = await _dep.SafeExecutor.ExecuteAsync(
            () => _dep.Db.Tenants.FirstOrDefaultAsync(t => t.AdminEmail == email),
            LogEventType.EmailSendFailure,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );

        if (!tenantResult.IsSuccess)
            return tenantResult.ToObjectResponse();

        var tenant = tenantResult.Response.Data;
        
        if(tenant != null)
            _dep.UserContext.Inject(tenantId: tenant.Id);
        
        var redisKey = RedisKeys.ResendAccountActivationEmail(email);
        var isLimited = await _dep.RedisLimiterService.IsLimitedAsync(redisKey);

        var guardResult = await _dep.GuardChecker
            .Check(() => tenant == null, LogEventType.EmailSendFailure, LogReasons.TenantNotFound, ErrorCodes.TenantNotFound)
            .Check(() => tenant!.IsEmailVerified, LogEventType.EmailSendFailure, LogReasons.TenantAccountAlreadyActivated, ErrorCodes.AccountAlreadyActivated)
            .CheckAsync(() => Task.FromResult(isLimited), LogEventType.EmailSendFailure, LogReasons.TooFrequentEmailRequest, ErrorCodes.TooFrequentEmailRequest)
            .ValidateAsync();

        if (guardResult != null)
            return guardResult;

        tenant.EmailVerificationToken = TokenGenerator.GenerateActivateAccountToken();
        tenant.LastEmailSentAt = DateTime.UtcNow;

        var saveResult = await _dep.SafeExecutor.ExecuteAsync(
            async () => { await _dep.Db.SaveChangesAsync(); },
            LogEventType.EmailSendFailure,
            LogReasons.DatabaseSaveFailed,
            ErrorCodes.DatabaseUnavailable
        );

        if (!saveResult.IsSuccess)
            return saveResult.ToObjectResponse();

        var payload = new ActivationEmailPayload
        {
            Email = tenant.AdminEmail,
            Token = tenant.EmailVerificationToken,
            Timestamp = DateTime.UtcNow.ToString("o")
        };
        
        _dep.UserContext.ApplyTo(payload);

        var sendResult = await _dep.SafeExecutor.ExecuteAsync(
            async () => { await _dep.EmailQueueProducer.SendAsync(payload); },
            LogEventType.EmailSendFailure,
            LogReasons.SendSqsMessageFailed,
            ErrorCodes.EmailSendFailed
        );

        if (!sendResult.IsSuccess)
            return sendResult.ToObjectResponse();

        await _dep.RedisLimiterService.SetLimitAsync(redisKey, TimeSpan.FromMinutes(_dep.AppOptions.EmailSendLimitMinutes));
        await _dep.LogDispatcher.Dispatch(LogEventType.EmailSendSuccess);

        return ApiResponse<object>.Ok(
            null,
            _dep.Localizer.GetSuccessMessage(SuccessCodes.EmailResent),
            traceId: _dep.UserContext.TraceId
        );
    }
}

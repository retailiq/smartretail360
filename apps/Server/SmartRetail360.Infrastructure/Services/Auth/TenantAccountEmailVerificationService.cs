using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.DTOs.AccountRegistration.Responses;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Application.Interfaces.Services;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Infrastructure.Services.Auth.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Exceptions;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Redis;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Auth;

public class TenantAccountEmailVerificationService : IEmailVerificationService
{
    private readonly AuthDependencies _dep;

    public TenantAccountEmailVerificationService(
        AuthDependencies dep)
    {
        _dep = dep;
    }

    public async Task<ApiResponse<object>> VerifyEmailAsync(string token)
    {
        var tenantResult = await _dep.SafeExecutor.ExecuteAsync(
            () => _dep.Db.Tenants.FirstOrDefaultAsync(t => t.EmailVerificationToken == token),
            LogEventType.AccountActivateFailure,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );

        if (!tenantResult.IsSuccess)
            return tenantResult.ToObjectResponse();

        var existingTenant = tenantResult.Response.Data;
        var redisKey = RedisKeys.VerifyEmailRateLimit(token);
        var isLimited = await _dep.RedisLimiterService.IsLimitedAsync(redisKey);

        var guardResult = await new GuardChecker(_dep.LogDispatcher, _dep.UserContext, _dep.Localizer)
            .WithEmail(existingTenant?.AdminEmail)
            .WithTenantId(existingTenant?.Id)
            .Check(() => existingTenant == null, LogEventType.AccountActivateFailure,
                LogReasons.InvalidTokenOrAccountAlreadyActivated, ErrorCodes.InvalidTokenOrAccountAlreadyActivated)
            .Check(() => isLimited, LogEventType.AccountActivateFailure, LogReasons.TooFrequentActivationAttempt,
                ErrorCodes.TooFrequentActivationAttempt)
            .ValidateAsync();
        
        if (guardResult != null)
            return guardResult;

        existingTenant.IsEmailVerified = true;
        existingTenant.EmailVerificationToken = null;
        existingTenant.StatusEnum = TenantStatus.Active;

        var saveResult = await _dep.SafeExecutor.ExecuteAsync(
            async () => { await _dep.Db.SaveChangesAsync(); },
            LogEventType.AccountActivateFailure,
            LogReasons.DatabaseSaveFailed,
            ErrorCodes.DatabaseUnavailable,
            email: existingTenant.AdminEmail,
            tenantId: existingTenant.Id
        );

        if (!saveResult.IsSuccess)
            return saveResult.ToObjectResponse();

        await _dep.RedisLimiterService.SetLimitAsync(redisKey,
            TimeSpan.FromMinutes(_dep.AppOptions.AccountActivationLimitMinutes));
        await _dep.LogDispatcher.Dispatch(LogEventType.AccountActivateSuccess, email: existingTenant.AdminEmail,
            tenantId: existingTenant.Id);

        return ApiResponse<object>.Ok(
            null,
            _dep.Localizer.GetSuccessMessage(SuccessCodes.AccountActivatedSuccessfully),
            traceId: _dep.UserContext.TraceId
        );
    }
}
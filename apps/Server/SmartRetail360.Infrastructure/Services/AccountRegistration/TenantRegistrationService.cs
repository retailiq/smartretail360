using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.DTOs.AccountRegistration.Requests;
using SmartRetail360.Application.DTOs.AccountRegistration.Responses;
using SmartRetail360.Application.Interfaces.AccountRegistration;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.DTOs.Messaging;
using SmartRetail360.Infrastructure.Services.AccountRegistration.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Redis;
using SmartRetail360.Shared.Responses;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Infrastructure.Services.AccountRegistration;

public class TenantRegistrationService : ITenantRegistrationService
{
    private readonly TenantRegistrationDependencies _dep;

    public TenantRegistrationService(TenantRegistrationDependencies dep)
    {
        _dep = dep;
    }

    public async Task<ApiResponse<TenantRegisterResponse>> RegisterTenantAsync(TenantRegisterRequest request)
    {
        var slug = SlugGenerator.GenerateSlug(request.AdminEmail);

        var traceId = _dep.UserContext.TraceId;
        if (string.IsNullOrWhiteSpace(traceId))
        {
            traceId = TraceIdGenerator.Generate(TraceIdPrefix.Get(TraceModule.Auth), slug);
        }

        var lockKey = RedisKeys.RegisterAccountLock(request.AdminEmail.ToLower());
        var lockAcquired = await _dep.RedisLockService.AcquireLockAsync(lockKey, TimeSpan.FromSeconds(_dep.AppOptions.RegistrationLockTtlSeconds));
        
        var lockCheck = await new GuardChecker(_dep.LogDispatcher, _dep.UserContext, _dep.Localizer)
            .WithEmail(request.AdminEmail)
            .Check(() => !lockAcquired, LogEventType.RegisterFailure, LogReasons.LockNotAcquired, ErrorCodes.DuplicateRegisterAttempt)
            .ValidateAsync();

        if (lockCheck != null)
            return lockCheck.To<TenantRegisterResponse>();
        
        // await Task.Delay(TimeSpan.FromSeconds(30));
        
        try
        {
            var tenantResult = await _dep.SafeExecutor.ExecuteAsync(
                () => _dep.Db.Tenants.FirstOrDefaultAsync(t => t.AdminEmail == request.AdminEmail),
                LogEventType.RegisterFailure,
                LogReasons.DatabaseOperationFailed,
                ErrorCodes.DatabaseUnavailable
            );

            if (!tenantResult.IsSuccess)
                return tenantResult.ToObjectResponse().To<TenantRegisterResponse>();

            var existingTenant = tenantResult.Response.Data;
            
            var guardResult = await new GuardChecker(_dep.LogDispatcher, _dep.UserContext, _dep.Localizer)
                .WithEmail(request.AdminEmail)
                .WithTenantId(existingTenant?.Id)
                .Check(() => existingTenant is { IsEmailVerified: true }, LogEventType.RegisterFailure, LogReasons.TenantAccountAlreadyExists, ErrorCodes.AccountAlreadyActivated)
                .Check(() => existingTenant is { IsEmailVerified: false }, LogEventType.RegisterFailure, LogReasons.TenantAccountExistsButNotActivated, ErrorCodes.AccountExistsButNotActivated)
                .ValidateAsync();

            if (guardResult != null)
                return guardResult.To<TenantRegisterResponse>();

            var passwordHash = PasswordHelper.HashPassword(request.Password);

            var tenant = new Tenant
            {
                AdminEmail = request.AdminEmail,
                Slug = slug,
                PasswordHash = passwordHash,
                TraceId = traceId,
                EmailVerificationToken = TokenGenerator.GenerateActivateAccountToken(),
                LastEmailSentAt = DateTime.UtcNow
            };

            var saveResult = await _dep.SafeExecutor.ExecuteAsync(
                async () => {
                    _dep.Db.Tenants.Add(tenant);
                    await _dep.Db.SaveChangesAsync();
                },
                LogEventType.RegisterFailure,
                LogReasons.DatabaseOperationFailed,
                ErrorCodes.DatabaseUnavailable
            );

            if (!saveResult.IsSuccess)
                return saveResult.ToObjectResponse().To<TenantRegisterResponse>();

            var emailResult = await _dep.SafeExecutor.ExecuteAsync(
                async () =>
                {
                    var payload = new ActivationEmailPayload
                    {
                        Email = tenant.AdminEmail,
                        Token = tenant.EmailVerificationToken,
                        TraceId = traceId,
                        TenantId = tenant.Id,
                        Locale = _dep.UserContext.Locale ?? "en",
                        Timestamp = DateTime.UtcNow.ToString("o")
                    };

                    await _dep.EmailQueueProducer.SendAsync(payload);
                },
                LogEventType.RegisterFailure,
                LogReasons.EmailSendFailed,
                ErrorCodes.EmailSendFailed
            );

            if (!emailResult.IsSuccess)
                return emailResult.ToObjectResponse().To<TenantRegisterResponse>();

            await _dep.LogDispatcher.Dispatch(LogEventType.RegisterSuccess, email: request.AdminEmail);

            return ApiResponse<TenantRegisterResponse>.Ok(
                new TenantRegisterResponse
                {
                    TenantId = tenant.Id,
                    Email = tenant.AdminEmail,
                    Status = tenant.Status,
                },
                _dep.Localizer.GetSuccessMessage(SuccessCodes.TenantRegistered),
                traceId
            );
        }
        finally
        {
            await _dep.RedisLockService.ReleaseLockAsync(lockKey);
        }
    }
}

// TODO: 创建初始用户（作为管理员）

// TODO: 初始化权限组/角色/默认设置

// TODO: 生成 AccessToken & RefreshToken
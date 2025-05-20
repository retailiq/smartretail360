using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.DTOs.AccountRegistration.Requests;
using SmartRetail360.Application.DTOs.AccountRegistration.Responses;
using SmartRetail360.Application.Extensions;
using SmartRetail360.Application.Interfaces.AccountRegistration;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Services.AccountRegistration.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.DTOs.Messaging;
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
        
        _dep.UserContext.Inject(clientEmail: request.AdminEmail, action:LogActions.TenantRegister);

        var lockKey = RedisKeys.RegisterAccountLock(request.AdminEmail.ToLower());
        var lockAcquired = await _dep.RedisLockService.AcquireLockAsync(lockKey, TimeSpan.FromSeconds(_dep.AppOptions.RegistrationLockTtlSeconds));
        
        var lockCheck = await _dep.GuardChecker
            .Check(() => !lockAcquired, LogEventType.RegisterFailure, LogReasons.LockNotAcquired, ErrorCodes.DuplicateRegisterAttempt)
            .ValidateAsync();

        if (lockCheck != null)
            return lockCheck.To<TenantRegisterResponse>();
        
        try
        {
            var tenantResult = await _dep.SafeExecutor.ExecuteAsync(
                () => _dep.Db.Tenants.FirstOrDefaultAsync(t => t.AdminEmail == request.AdminEmail),
                LogEventType.RegisterFailure,
                LogReasons.DatabaseRetrievalFailed,
                ErrorCodes.DatabaseUnavailable
            );

            if (!tenantResult.IsSuccess)
                return tenantResult.ToObjectResponse().To<TenantRegisterResponse>();

            var existingTenant = tenantResult.Response.Data;
            
            if (existingTenant != null)
            {
                _dep.UserContext.Inject(tenantId: existingTenant.Id);
            }
            
            var guardResult = await _dep.GuardChecker
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
            
            _dep.UserContext.Inject(tenantId: tenant.Id);

            var saveResult = await _dep.SafeExecutor.ExecuteAsync(
                async () => {
                    _dep.Db.Tenants.Add(tenant);
                    await _dep.Db.SaveChangesAsync();
                },
                LogEventType.RegisterFailure,
                LogReasons.DatabaseSaveFailed,
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
                        Timestamp = DateTime.UtcNow.ToString("o")
                        
                    };
                    
                    _dep.UserContext.ApplyTo(payload);

                    await _dep.EmailQueueProducer.SendAsync(payload);
                },
                LogEventType.RegisterFailure,
                LogReasons.SendSqsMessageFailed,
                ErrorCodes.EmailSendFailed
            );

            if (!emailResult.IsSuccess)
                return emailResult.ToObjectResponse().To<TenantRegisterResponse>();

            await _dep.LogDispatcher.Dispatch(LogEventType.RegisterSuccess);

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
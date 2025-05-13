using System.Net;
using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.DTOs.AccountRegistration.Requests;
using SmartRetail360.Application.DTOs.AccountRegistration.Responses;
using SmartRetail360.Application.Interfaces.AccountRegistration;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Services.AccountRegistration.Models;
using SmartRetail360.Shared.Catalogs;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Exceptions;
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
        _dep.UserContext.LogAllContext();

        var slug = SlugGenerator.GenerateSlug(request.AdminEmail);

        var traceId = _dep.UserContext.TraceId;
        if (string.IsNullOrWhiteSpace(traceId))
        {
            traceId = TraceIdGenerator.Generate(TraceIdPrefix.Get(TraceModule.Auth), slug);
        }

        var lockKey = RedisKeys.RegisterAccountLock(request.AdminEmail.ToLower());
        var lockAcquired =
            await _dep.LockService.AcquireLockAsync(lockKey, TimeSpan.FromSeconds(_dep.AppOptions.RegistrationLockTtlSeconds));
        if (!lockAcquired)
        {
            await _dep.AuditLogger.LogLockFailedAsync(request.AdminEmail);
            throw new CommonException(ErrorCodes.DuplicateRegisterAttempt);
        }

        try
        {
            var existingTenant = await _dep.Db.Tenants.FirstOrDefaultAsync(t => t.AdminEmail == request.AdminEmail);
            if (existingTenant != null)
            {
                await _dep.AuditLogger.LogAccountExistsAsync(request.AdminEmail);
                throw new CommonException(ErrorCodes.AccountExists, HttpStatusCode.Conflict);
            }

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
            // await Task.Delay(TimeSpan.FromSeconds(30));
            _dep.Db.Tenants.Add(tenant);
            await _dep.Db.SaveChangesAsync();

            var variables = new Dictionary<string, string>
            {
                ["traceId"] = _dep.UserContext.TraceId ?? string.Empty,
                ["tenantId"] = tenant.Id.ToString(),
                ["locale"] = _dep.UserContext.Locale ?? "en",
                ["token"] = tenant.EmailVerificationToken,
                ["timestamp"] = DateTime.UtcNow.ToString("o")
            };

            try
            {
                await _dep.EmailContext.SendAsync(
                    EmailTemplate.TenantAccountActivation,
                    toEmail: tenant.AdminEmail,
                    variables: variables
                );
            }
            catch (Exception ex)
            {
                await _dep.AuditLogger.LogEmailFailedAsync(request.AdminEmail, ex.Message);
                throw new CommonException(ErrorCodes.EmailSendFailed, HttpStatusCode.ServiceUnavailable);
            }
            
            await _dep.AuditLogger.LogRegisterSuccessAsync(tenant.AdminEmail);
            
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
            await _dep.LockService.ReleaseLockAsync(lockKey);
        }
    }
}

// TODO: 创建初始用户（作为管理员）

// TODO: 初始化权限组/角色/默认设置

// TODO: 生成 AccessToken & RefreshToken
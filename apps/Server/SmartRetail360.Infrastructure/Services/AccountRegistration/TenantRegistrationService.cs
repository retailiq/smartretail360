using Microsoft.EntityFrameworkCore;
using Npgsql;
using SmartRetail360.Application.DTOs.AccountRegistration.Requests;
using SmartRetail360.Application.DTOs.AccountRegistration.Responses;
using SmartRetail360.Application.Interfaces.AccountRegistration;
using SmartRetail360.Domain.Entities;
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
        var lockAcquired =
            await _dep.RedisLockService.AcquireLockAsync(lockKey,
                TimeSpan.FromSeconds(_dep.AppOptions.RegistrationLockTtlSeconds));
        if (!lockAcquired)
        {
            await _dep.LogDispatcher.Dispatch(
                LogEventType.RegisterFailure,
                reason: LogReasons.LockNotAcquired,
                email: request.AdminEmail
            );
            return ApiResponse<TenantRegisterResponse>.Fail(
                ErrorCodes.DuplicateRegisterAttempt,
                _dep.Localizer.GetErrorMessage(ErrorCodes.DuplicateRegisterAttempt),
                traceId
            );
        }

        // await Task.Delay(TimeSpan.FromSeconds(30));

        try
        {
            var existingTenant = await _dep.Db.Tenants.FirstOrDefaultAsync(t => t.AdminEmail == request.AdminEmail);
            if (existingTenant is { IsEmailVerified: true })
            {
                await _dep.LogDispatcher.Dispatch(
                    LogEventType.RegisterFailure,
                    reason: LogReasons.TenantAccountAlreadyExists,
                    email: request.AdminEmail
                );

                // await _dep.LogDispatcher.Dispatch(
                //     LogEventType.LoginFailure,
                //     reason: LogReasons.InvalidCredentials,
                //     email: request.AdminEmail
                // );

                return ApiResponse<TenantRegisterResponse>.Fail(
                    ErrorCodes.AccountAlreadyActivated,
                    _dep.Localizer.GetErrorMessage(ErrorCodes.AccountAlreadyActivated),
                    traceId
                );
            }
            
            if (existingTenant is { IsEmailVerified: false })
            {
                await _dep.LogDispatcher.Dispatch(
                    LogEventType.RegisterFailure,
                    reason: LogReasons.TenantAccountExistsButNotActivated,
                    email: request.AdminEmail
                );
                
                return ApiResponse<TenantRegisterResponse>.Fail(
                    ErrorCodes.AccountExistsButNotActivated,
                    _dep.Localizer.GetErrorMessage(ErrorCodes.AccountExistsButNotActivated),
                    traceId
                );
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
            // _dep.Db.Tenants.Add(tenant);
            // await _dep.Db.SaveChangesAsync();

            try
            {
                _dep.Db.Tenants.Add(tenant);
                await _dep.Db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
            {
                await _dep.LogDispatcher.Dispatch(
                    LogEventType.RegisterFailure,
                    reason: LogReasons.DatabaseOperationFailed,
                    errorStack: pgEx.Message,
                    email: request.AdminEmail
                );

                return ApiResponse<TenantRegisterResponse>.Fail(
                    ErrorCodes.DatabaseUnavailable,
                    _dep.Localizer.GetErrorMessage(ErrorCodes.DatabaseUnavailable),
                    traceId
                );
            }


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
                await _dep.LogDispatcher.Dispatch(
                    LogEventType.RegisterFailure,
                    reason: LogReasons.EmailSendFailed,
                    errorStack: ex.Message,
                    email: request.AdminEmail
                );

                return ApiResponse<TenantRegisterResponse>.Fail(
                    ErrorCodes.EmailSendFailed,
                    _dep.Localizer.GetErrorMessage(ErrorCodes.EmailSendFailed),
                    traceId
                );
            }

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
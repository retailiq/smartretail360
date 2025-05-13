using System.Net;
using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.DTOs.AccountRegistration.Requests;
using SmartRetail360.Application.DTOs.AccountRegistration.Responses;
using SmartRetail360.Application.Interfaces.AccountRegistration;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Application.Interfaces.Services;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Services.Notifications.Configuration;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Exceptions;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Options;
using SmartRetail360.Shared.Redis;
using SmartRetail360.Shared.Responses;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Infrastructure.Services.AccountRegistration;

public class TenantRegistrationService : ITenantRegistrationService
{
    private readonly AppDbContext _db;
    private readonly IUserContextService _userContext;
    private readonly MessageLocalizer _localizer;
    private readonly EmailContext _emailContext;
    private readonly ILockService _lockService;
    private readonly AppOptions _appOptions;
    private readonly IAuditLogger _auditLogger;

    public TenantRegistrationService(
        AppDbContext db,
        IUserContextService userContext,
        MessageLocalizer localizer,
        EmailContext emailContext,
        ILockService lockService,
        AppOptions appOptions,
        IAuditLogger auditLogger
    )
    {
        _db = db;
        _userContext = userContext;
        _localizer = localizer;
        _emailContext = emailContext;
        _lockService = lockService;
        _appOptions = appOptions;
        _auditLogger = auditLogger;
    }

    public async Task<ApiResponse<TenantRegisterResponse>> RegisterTenantAsync(TenantRegisterRequest request)
    {
        _userContext.LogAllContext();

        // Generate a Slug
        var slug = SlugGenerator.GenerateSlug(request.AdminEmail);

        var lockKey = RedisKeys.RegisterAccountLock(request.AdminEmail.ToLower());
        var lockAcquired = await _lockService.AcquireLockAsync(lockKey, TimeSpan.FromSeconds(_appOptions.RegistrationLockTtlSeconds));
        if (!lockAcquired)
            throw new CommonException(ErrorCodes.DuplicateRegisterAttempt);

        try
        {
            // Handle the trace ID
            var traceId = _userContext.TraceId;
            if (string.IsNullOrWhiteSpace(traceId))
            {
                traceId = TraceIdGenerator.Generate(TraceIdPrefix.Get(TraceModule.Auth), slug);
            }

            // Check if the AdminEmail is existing (to avoid duplicate registration)
            var existingTenant = await _db.Tenants.FirstOrDefaultAsync(t => t.AdminEmail == request.AdminEmail);

            if (existingTenant != null) throw new CommonException(ErrorCodes.AccountExists, HttpStatusCode.Conflict);

            // Hash the password
            var passwordHash = PasswordHelper.HashPassword(request.Password);

            // Create a new tenant
            var tenant = new Tenant
            {
                AdminEmail = request.AdminEmail,
                Slug = slug,
                PasswordHash = passwordHash,
                TraceId = traceId,
                EmailVerificationToken = TokenGenerator.GenerateActivateAccountToken(),
                LastEmailSentAt = DateTime.UtcNow
            };
            // await Task.Delay(TimeSpan.FromSeconds(10));
            _db.Tenants.Add(tenant);
            await _db.SaveChangesAsync();

            // Send the activation email
            var variables = new Dictionary<string, string>
            {
                ["traceId"] = _userContext.TraceId ?? string.Empty,
                ["tenantId"] = tenant.Id.ToString(),
                ["locale"] = _userContext.Locale ?? "en",
                ["token"] = tenant.EmailVerificationToken,
                ["timestamp"] = DateTime.UtcNow.ToString("o")
            };

            await _emailContext.SendAsync(
                EmailTemplate.TenantAccountActivation,
                toEmail: tenant.AdminEmail,
                variables: variables
            );
            
            await _auditLogger.LogAsync(new AuditLog
            {
                TenantId = tenant.Id,
                Action = AuditActions.RegisterTenant,
                TraceId = traceId,
                EvaluatedAt = DateTime.UtcNow,
                IsSuccess = true,
                UnserializedDetails = new Dictionary<string, string>
                {
                    { "Email", request.AdminEmail },
                    { "SourceIp", _userContext.IpAddress ?? string.Empty }
                }
            });

            return ApiResponse<TenantRegisterResponse>.Ok(
                new TenantRegisterResponse
                {
                    TenantId = tenant.Id,
                    Email = tenant.AdminEmail,
                    Status = tenant.Status,
                },
                _localizer.GetSuccessMessage(SuccessCodes.TenantRegistered),
                traceId
            );
        }
        finally
        {
            await _lockService.ReleaseLockAsync(lockKey);
        }
    }
}

// TODO: 创建初始用户（作为管理员）

// TODO: 初始化权限组/角色/默认设置

// TODO: 生成 AccessToken & RefreshToken
using System.Net;
using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.DTOs.Auth.Requests;
using SmartRetail360.Application.DTOs.Auth.Responses;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Application.Interfaces.Notifications;
using SmartRetail360.Application.Interfaces.Notifications.Strategies;
using SmartRetail360.Application.Interfaces.TenantManagement;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Services.Notifications;
using SmartRetail360.Infrastructure.Services.Notifications.Configuration;
using SmartRetail360.Infrastructure.Services.Notifications.Strategies;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Exceptions;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Responses;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Infrastructure.Services.TenantManagement;

public class TenantRegistrationService : ITenantRegistrationService
{
    private readonly AppDbContext _db;
    private readonly IUserContextService _userContext;
    private readonly MessageLocalizer _localizer;
    private readonly EmailContext _emailContext;
    private readonly IEmailStrategy _activationStrategy;

    public TenantRegistrationService(
        AppDbContext db, 
        IUserContextService userContext, 
        MessageLocalizer localizer,
        IEmailNotificationService emailNotificationService, 
        EmailContext emailContext,
        AccountActivationEmailStrategy activationStrategy)
    {
        _db = db;
        _userContext = userContext;
        _localizer = localizer;
        _emailContext = emailContext;
        _activationStrategy = activationStrategy;
    }

    public async Task<ApiResponse<TenantRegisterResponse>> RegisterTenantAsync(TenantRegisterRequest request)
    {
        _userContext.LogAllContext();

        // Generate a Slug
        var slug = GenerateSlug(request.AdminEmail);

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
        _db.Tenants.Add(tenant);
        await _db.SaveChangesAsync();

        // Send activation email
        _emailContext.SetStrategy(_activationStrategy);
        await _emailContext.ExecuteAsync(tenant);

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

    private string GenerateSlug(string email)
    {
        return email.Split('@')[0].ToLower().Replace(".", "-").Replace("_", "-");
    }
}

// TODO: 创建初始用户（作为管理员）

// TODO: 初始化权限组/角色/默认设置

// TODO: 生成 AccessToken & RefreshToken
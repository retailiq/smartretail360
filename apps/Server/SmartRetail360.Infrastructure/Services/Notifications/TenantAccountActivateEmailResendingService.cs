using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Application.Interfaces.Notifications;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Exceptions;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Options;
using SmartRetail360.Shared.Utils;
using Microsoft.Extensions.Options;
using SmartRetail360.Application.Interfaces.Notifications.Strategies;
using SmartRetail360.Application.Interfaces.Services;
using SmartRetail360.Infrastructure.Services.Notifications.Configuration;
using SmartRetail360.Infrastructure.Services.Notifications.Strategies;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Redis;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Notifications;

public class TenantAccountActivateEmailResendingService : IAccountActivateEmailResendingService
{
    private readonly AppDbContext _dbContext;
    private readonly AppOptions _appOptions;
    private readonly IUserContextService _userContext;
    private readonly MessageLocalizer _localizer;
    private readonly EmailContext _emailContext;
    private readonly ILimiterService _limiterService;
    
    public TenantAccountActivateEmailResendingService(
        AppDbContext dbContext,
        IOptions<AppOptions> options,
        IUserContextService userContext,
        MessageLocalizer localizer,
        EmailContext emailContext,
        ILimiterService limiterService
        )
    {
        _dbContext = dbContext;
        _appOptions = options.Value;
        _userContext = userContext;
        _localizer = localizer;
        _emailContext = emailContext;
        _limiterService = limiterService;
    }

    public async Task<ApiResponse<object>> ResendEmailAsync(string email)
    {
        var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(t => t.AdminEmail == email);
        
        if (tenant == null) throw new SecurityException(ErrorCodes.TenantNotFound);
        if (tenant.IsEmailVerified) throw new CommonException(ErrorCodes.AccountAlreadyActivated);
        
        var redisKey = RedisKeys.ResendAccountActivationEmail(email);
        if (await _limiterService.IsLimitedAsync(redisKey))
            throw new CommonException(ErrorCodes.TooFrequentEmailRequest);

        tenant.EmailVerificationToken = TokenGenerator.GenerateActivateAccountToken();
        tenant.LastEmailSentAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        
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
        
        await _limiterService.SetLimitAsync(redisKey, TimeSpan.FromMinutes(_appOptions.EmailSendLimitMinutes));
        
        return ApiResponse<object>.Ok(
            null,
            _localizer.GetSuccessMessage(SuccessCodes.EmailResent),
            traceId: _userContext.TraceId
        );
    }
}


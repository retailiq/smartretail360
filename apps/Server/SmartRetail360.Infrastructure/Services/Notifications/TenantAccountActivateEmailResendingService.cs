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
using SmartRetail360.Infrastructure.Services.Notifications.Configuration;
using SmartRetail360.Infrastructure.Services.Notifications.Strategies;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Notifications;

public class TenantAccountActivateEmailResendingService : IAccountActivateEmailResendingService
{
    private readonly AppDbContext _dbContext;
    private readonly AppOptions _appOptions;
    private readonly IUserContextService _userContext;
    private readonly MessageLocalizer _localizer;
    private readonly EmailContext _emailContext;
    private readonly IEmailStrategy _activationStrategy;
    
    public TenantAccountActivateEmailResendingService(
        AppDbContext dbContext,
        IOptions<AppOptions> options,
        IUserContextService userContext,
        MessageLocalizer localizer,
        EmailContext emailContext,
        TenantAccountActivationEmailStrategy activationStrategy)
    {
        _dbContext = dbContext;
        _appOptions = options.Value;
        _userContext = userContext;
        _localizer = localizer;
        _emailContext = emailContext;
        _activationStrategy = activationStrategy;
    }

    public async Task<ApiResponse<object>> ResendEmailAsync(string email)
    {
        var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(t => t.AdminEmail == email);
        var limitMinutes = int.Parse(_appOptions.EmailSendLimitMinutes);
        
        if (tenant == null) throw new SecurityException(ErrorCodes.TenantNotFound);
        if (tenant.IsEmailVerified) throw new CommonException(ErrorCodes.AccountAlreadyActivated);
        if (tenant.LastEmailSentAt.HasValue &&
            (DateTime.UtcNow - tenant.LastEmailSentAt.Value).TotalMinutes < limitMinutes)
        {
            throw new CommonException(ErrorCodes.TooFrequentEmailRequest);
        }

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
        
        return ApiResponse<object>.Ok(
            null,
            _localizer.GetSuccessMessage(SuccessCodes.EmailResent),
            traceId: _userContext.TraceId
        );
    }
}


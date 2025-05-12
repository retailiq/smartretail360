using SmartRetail360.Application.Interfaces.Notifications;
using SmartRetail360.Application.Interfaces.Notifications.Configuration;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Exceptions;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Notifications.Configuration;

public class EmailVerificationDispatchService : IEmailDispatchService
{
    private readonly IAccountActivateEmailResendingService _resendingTenantAccountActivateEmailService;

    public EmailVerificationDispatchService(
        IAccountActivateEmailResendingService resendingTenantAccountActivateEmailService
    )
    {
        _resendingTenantAccountActivateEmailService = resendingTenantAccountActivateEmailService;
    }

    public async Task<ApiResponse<object>> DispatchAsync(EmailTemplate template, string email)
    {
        return template switch
        {
            EmailTemplate.TenantAccountActivation => await _resendingTenantAccountActivateEmailService.ResendEmailAsync(email),
            _ => throw new CommonException(ErrorCodes.UnsupportedEmailTemplate)
        };
    }
}
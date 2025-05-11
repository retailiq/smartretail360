using SmartRetail360.Application.Interfaces.Notifications;
using SmartRetail360.Application.Interfaces.Notifications.Configuration;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Exceptions;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Notifications.Configuration;

public class EmailDispatchService : IEmailDispatchService
{
    private readonly IAccountActivateEmailResendingService _resendingAccountActivateEmailService;

    public EmailDispatchService(
        IAccountActivateEmailResendingService resendingAccountActivateEmailService
    )
    {
        _resendingAccountActivateEmailService = resendingAccountActivateEmailService;
    }

    public async Task<ApiResponse<object>> DispatchAsync(EmailTemplate template, string email)
    {
        return template switch
        {
            EmailTemplate.AccountActivation => await _resendingAccountActivateEmailService.ResendEmailAsync(email),
            _ => throw new CommonException(ErrorCodes.UnsupportedEmailTemplate)
        };
    }
}
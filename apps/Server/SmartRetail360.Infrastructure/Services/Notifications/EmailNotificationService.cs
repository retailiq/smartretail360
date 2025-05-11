using SmartRetail360.Application.Interfaces.Notifications;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Notifications;

public class EmailNotificationService : IEmailNotificationService
{
    private readonly IAccountActivateEmailResendingService _accountActivateEmailResendingService;

    public EmailNotificationService(IAccountActivateEmailResendingService accountActivateEmailResendingService)
    {
        _accountActivateEmailResendingService = accountActivateEmailResendingService;
    }
    
    public async Task<ApiResponse<object>> ResendAccountActivateEmailAsync(string adminEmail)
        => await _accountActivateEmailResendingService.ResendEmailAsync(adminEmail);
}
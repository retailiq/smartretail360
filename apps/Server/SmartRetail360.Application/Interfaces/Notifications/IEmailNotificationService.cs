using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Application.Interfaces.Notifications;

public interface IEmailNotificationService
{
    Task<ApiResponse<object>> ResendAccountActivateEmailAsync(string adminEmail);
}
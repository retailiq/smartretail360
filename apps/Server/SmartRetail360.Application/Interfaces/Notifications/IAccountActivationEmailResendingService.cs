using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Application.Interfaces.Notifications;

public interface IAccountActivationEmailResendingService
{
    Task<ApiResponse<object>> ResendEmailAsync(string email);
}
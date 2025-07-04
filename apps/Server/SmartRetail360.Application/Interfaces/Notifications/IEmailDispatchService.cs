using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Application.Interfaces.Notifications;

public interface IEmailDispatchService
{
    Task<ApiResponse<object>> DispatchAsync(EmailTemplate template, string email);
}
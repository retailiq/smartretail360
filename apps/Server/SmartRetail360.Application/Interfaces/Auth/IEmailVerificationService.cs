using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Application.Interfaces.Auth;

public interface IEmailVerificationService
{
    Task<ApiResponse<object>> VerifyEmailAsync(string token);
}
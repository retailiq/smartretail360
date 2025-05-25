using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Application.Interfaces.Auth;

public interface IAccountEmailVerificationService
{
    Task<ApiResponse<object>> VerifyEmailAsync(string token);
}
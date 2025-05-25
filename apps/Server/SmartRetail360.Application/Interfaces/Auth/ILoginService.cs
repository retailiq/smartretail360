using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Application.Interfaces.Auth;

public interface ILoginService
{
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
}
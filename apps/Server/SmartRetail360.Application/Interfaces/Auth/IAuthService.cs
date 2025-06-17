using Microsoft.AspNetCore.Identity.Data;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Application.Interfaces.Auth;

public interface IAuthService
{
    Task<ApiResponse<RefreshTokenResponse>> RefreshAsync(RefreshTokenRequest request);
    Task<ApiResponse<object>> LogoutAsync();
    Task<ApiResponse<object>> ValidateToken();
}
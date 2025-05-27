using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Auth.OAuthLogin;

public class OAuthLoginService : IOAuthLoginService
{
    public OAuthLoginService()
    {}

    public Task<ApiResponse<LoginResponse>> OAuthLoginAsync(OAuthLoginRequest request)
    {
        throw new NotImplementedException();
    }
}
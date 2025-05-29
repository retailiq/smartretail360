using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Infrastructure.Services.Auth.Login.OAuthLogin.Models;

namespace SmartRetail360.Infrastructure.Services.Auth.Login.OAuthLogin.Handlers;

public interface IOAuthProviderHandler
{
    Task<OAuthUserInfo> GetUserProfileAsync(OAuthLoginRequest request);
}
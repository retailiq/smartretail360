using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Infrastructure.Services.Auth.OAuthLogin.Models;

namespace SmartRetail360.Infrastructure.Services.Auth.OAuthLogin.Handlers;

public interface IOAuthProviderHandler
{
    Task<OAuthUserProfileResult> GetUserProfileAsync(OAuthLoginRequest request);
}
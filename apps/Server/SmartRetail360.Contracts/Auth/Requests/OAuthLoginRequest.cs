using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Contracts.Auth.Requests;

public class OAuthLoginRequest
{
    public OAuthProvider Provider { get; set; }
    public string Code { get; set; } = string.Empty;
    public string OAuthState { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
}
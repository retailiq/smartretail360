using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Infrastructure.Services.Auth.OAuthLogin.Models;

public class OAuthUserInfo
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string ProviderUserId { get; set; } = string.Empty;
    public OAuthProvider Provider { get; set; }
}
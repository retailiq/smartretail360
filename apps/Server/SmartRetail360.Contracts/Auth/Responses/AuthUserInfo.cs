namespace SmartRetail360.Contracts.Auth.Responses;

public class AuthUserInfo
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public string Locale { get; set; } = "en-US";
    public string Name { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public bool IsFirstLogin { get; set; } = false;
    public IEnumerable<string> Permissions { get; set; } = [];
}
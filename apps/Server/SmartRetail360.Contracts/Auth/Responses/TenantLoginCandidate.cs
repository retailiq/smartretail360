namespace SmartRetail360.Contracts.Auth.Responses;

public class TenantLoginCandidate
{
    public string TenantId { get; set; } = string.Empty;
    public string TenantName { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public bool IsDefault { get; set; } = false;
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; } = true;
}
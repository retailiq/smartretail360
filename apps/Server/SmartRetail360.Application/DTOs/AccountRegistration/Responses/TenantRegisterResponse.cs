namespace SmartRetail360.Application.DTOs.AccountRegistration.Responses;

public class TenantRegisterResponse
{
    public Guid TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}
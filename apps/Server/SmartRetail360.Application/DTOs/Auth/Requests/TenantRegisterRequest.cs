namespace SmartRetail360.Application.DTOs.Auth.Requests;

public class TenantRegisterRequest
{
    public string AdminEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PasswordConfirmation { get; set; } = string.Empty;
}
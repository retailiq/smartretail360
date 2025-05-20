namespace SmartRetail360.Contracts.AccountRegistration.Requests;

public class TenantRegisterRequest
{
    public string AdminEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PasswordConfirmation { get; set; } = string.Empty;
}
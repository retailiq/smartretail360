namespace SmartRetail360.Contracts.AccountRegistration.Requests;

public class AccountRegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PasswordConfirmation { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
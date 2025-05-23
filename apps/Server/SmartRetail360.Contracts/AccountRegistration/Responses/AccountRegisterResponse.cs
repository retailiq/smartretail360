namespace SmartRetail360.Contracts.AccountRegistration.Responses;

public class AccountRegisterResponse
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}
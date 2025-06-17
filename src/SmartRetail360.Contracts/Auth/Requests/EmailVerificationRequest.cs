namespace SmartRetail360.Contracts.Auth.Requests;

public class EmailVerificationRequest
{
    public string Token { get; set; } = string.Empty;
}
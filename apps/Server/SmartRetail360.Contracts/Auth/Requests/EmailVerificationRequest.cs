using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Contracts.Auth.Requests;

public class EmailVerificationRequest
{
    public string Token { get; set; } = string.Empty;
    public AccountType Type { get; set; }
}
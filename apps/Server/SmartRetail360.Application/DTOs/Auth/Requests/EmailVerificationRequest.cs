using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.DTOs.Auth.Requests;

public class EmailVerificationRequest
{
    public string Token { get; set; } = string.Empty;
    public AccountType Type { get; set; }
}
using SmartRetail360.Shared.Constants;

namespace SmartRetail360.Contracts.Auth.Responses;

public class ConfirmTenantLoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = GeneralConstants.Bearer;
    public int ExpiresIn { get; set; } // in seconds
    public AuthUserInfo User { get; set; } = new();
}
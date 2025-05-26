namespace SmartRetail360.Contracts.Auth.Responses;

public class LoginResponse
{
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public bool? ShouldChooseTenant { get; set; }
    public bool? ShouldShowResendButton { get; set; }
    public int? LoginFailureCount { get; set; }
    public List<TenantLoginCandidate>? TenantOptions { get; set; }
}
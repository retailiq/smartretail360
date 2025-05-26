namespace SmartRetail360.Contracts.Auth.Requests;

public class ConfirmTenantLoginRequest
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
}
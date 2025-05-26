namespace SmartRetail360.Contracts.Auth.Requests;

public class ConfirmTenantLoginRequest
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public bool? IsStaySignedIn { get; set; }
}

// IsStaySignedIn must be nullable to allow for the case where the user does not specify this option.
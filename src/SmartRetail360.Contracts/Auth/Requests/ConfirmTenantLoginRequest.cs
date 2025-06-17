namespace SmartRetail360.Contracts.Auth.Requests;

public class ConfirmTenantLoginRequest
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public bool? IsStaySignedIn { get; set; }
}

// Must be nullable, because ASP.NET Core will set it tp false during model binding if not provided.
// // Use fluent validation to ensure it is not null.
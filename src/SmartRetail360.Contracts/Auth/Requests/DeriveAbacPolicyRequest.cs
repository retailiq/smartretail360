namespace SmartRetail360.Contracts.Auth.Requests;

public class DeriveAbacPolicyRequest
{
    public Guid TenantId { get; set; }
    public string TemplateName { get; set; } = default!;
}
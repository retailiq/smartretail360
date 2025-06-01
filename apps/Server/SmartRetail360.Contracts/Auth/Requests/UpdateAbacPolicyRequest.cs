namespace SmartRetail360.Contracts.Auth.Requests;

public class UpdateAbacPolicyRequest
{
    public Guid Id { get; set; }
    public string RuleJson { get; set; } = string.Empty;
}
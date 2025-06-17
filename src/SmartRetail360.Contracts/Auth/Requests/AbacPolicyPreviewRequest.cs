using System.ComponentModel.DataAnnotations;

namespace SmartRetail360.Contracts.Auth.Requests;

public class AbacPolicyPreviewRequest
{
    [Required]
    public string RuleJson { get; set; } = string.Empty;

    [Required]
    public object Context { get; set; } = default!;
}
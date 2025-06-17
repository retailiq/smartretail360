using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.ABAC.Interfaces.AbacPolicyService;

public interface IAbacPolicyUpdateService
{
    Task<ApiResponse<AbacPolicy>> UpdatePolicyRuleJsonAsync(Guid policyId, UpdateAbacPolicyRuleJsonRequest request);
    Task<ApiResponse<AbacPolicy>> UpdatePolicyStatusAsync(Guid policyId, UpdateAbacPolicyStatusRequest request);
}
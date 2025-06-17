using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.ABAC.Interfaces.AbacPolicyService;

public interface IAbacPolicyService
{
    Task<ApiResponse<List<AbacPolicy>>> GetAllPoliciesForTenantAsync(Guid tenantId);
    Task<ApiResponse<AbacPolicy>> UpdatePolicyRuleJsonAsync(Guid policyId, UpdateAbacPolicyRuleJsonRequest request);
    Task<ApiResponse<AbacPolicy>> UpdatePolicyStatusAsync(Guid policyId, UpdateAbacPolicyStatusRequest request);
    Task CreateDefaultPoliciesForTenantAsync(Guid tenantId, bool enableImmediately = false);
    Task<int> ApplyPolicyTemplatesToTenantAsync(Guid tenantId);
    Task CreatePoliciesFromTemplateAsync(Guid tenantId, string templateName);
    Task<ApiResponse<object>> DerivePolicyFromTemplateAsync(DeriveAbacPolicyRequest request);
    Task<int> SyncPolicyFromTemplateAsync(Guid templateId, string newRuleJson);
}
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Application.Interfaces.Auth.AccessControl;

public interface IAbacPolicyService
{
    Task<ApiResponse<List<AbacPolicy>>> GetAllPoliciesForTenantAsync(Guid tenantId);
    Task<ApiResponse<object>> UpdatePolicyAsync(UpdateAbacPolicyRequest request);
    Task CreateDefaultPoliciesForTenantAsync(Guid tenantId);
    Task<int> ApplyPolicyTemplatesToTenantAsync(Guid tenantId);
    Task CreatePoliciesFromTemplateAsync(Guid tenantId, string templateName);
    Task<ApiResponse<object>> DerivePolicyFromTemplateAsync(DeriveAbacPolicyRequest request);
    Task<int> SyncPolicyFromTemplateAsync(Guid templateId, string newRuleJson);
}
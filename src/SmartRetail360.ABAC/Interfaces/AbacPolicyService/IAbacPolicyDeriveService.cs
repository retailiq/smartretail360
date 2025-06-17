using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.ABAC.Interfaces.AbacPolicyService;

public interface IAbacPolicyDeriveService
{
    Task<ApiResponse<object>> DerivePolicyFromTemplateAsync(DeriveAbacPolicyRequest request);
}
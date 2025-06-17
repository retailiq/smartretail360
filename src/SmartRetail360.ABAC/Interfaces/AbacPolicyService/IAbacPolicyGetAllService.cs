using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.ABAC.Interfaces.AbacPolicyService;

public interface IAbacPolicyGetAllService
{
    Task<ApiResponse<List<AbacPolicy>>> GetAllPoliciesForTenantAsync(Guid tenantId);
}
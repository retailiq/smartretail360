namespace SmartRetail360.ABAC.Interfaces.AbacPolicyService;

public interface IAbacPolicyDefaultCreateService
{
    Task CreateDefaultPoliciesForTenantAsync(Guid tenantId, bool enableImmediately = false);
}
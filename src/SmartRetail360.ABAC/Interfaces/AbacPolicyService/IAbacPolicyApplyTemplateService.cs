namespace SmartRetail360.ABAC.Interfaces.AbacPolicyService;

public interface IAbacPolicyApplyTemplateService
{
    Task<int> ApplyPolicyTemplatesToTenantAsync(Guid tenantId);
}
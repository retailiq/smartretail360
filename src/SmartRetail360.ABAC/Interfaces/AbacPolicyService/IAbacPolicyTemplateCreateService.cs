namespace SmartRetail360.ABAC.Interfaces.AbacPolicyService;

public interface IAbacPolicyTemplateCreateService
{
    Task CreatePoliciesFromTemplateAsync(Guid tenantId, string templateName);
}
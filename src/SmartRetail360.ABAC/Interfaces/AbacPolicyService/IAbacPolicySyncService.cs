namespace SmartRetail360.ABAC.Interfaces.AbacPolicyService;

public interface IAbacPolicySyncService
{
    Task<int> SyncPolicyFromTemplateAsync(Guid templateId, string newRuleJson);
}
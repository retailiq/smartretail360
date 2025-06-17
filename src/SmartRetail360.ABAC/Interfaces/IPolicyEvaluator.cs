namespace SmartRetail360.ABAC.Interfaces;

public interface IPolicyEvaluator
{
    Task<bool> EvaluateAsync(Guid tenantId, string resourceType, string action, object context);
}
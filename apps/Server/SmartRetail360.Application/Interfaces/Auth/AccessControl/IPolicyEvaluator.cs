namespace SmartRetail360.Application.Interfaces.Auth.AccessControl;

public interface IPolicyEvaluator
{
    Task<bool> EvaluateAsync(Guid tenantId, string resourceType, string action, object context);
}
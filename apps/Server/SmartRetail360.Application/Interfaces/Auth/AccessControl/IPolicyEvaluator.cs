namespace SmartRetail360.Application.Interfaces.Auth.AccessControl;

public interface IPolicyEvaluator
{
    Task<bool> EvaluateAsync(string resourceType, string action, object context);
}
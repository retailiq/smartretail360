namespace SmartRetail360.ABAC.Interfaces;

public interface ICustomResourceResolver
{
    bool CanResolve(string resourceType);
    Task<Dictionary<string, object>> ResolveAsync(string? resourceId);
}
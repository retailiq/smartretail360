namespace SmartRetail360.Application.Interfaces.Auth.AccessControl;

public interface ICustomResourceResolver
{
    bool CanResolve(string resourceType);
    Task<Dictionary<string, object>> ResolveAsync(string? resourceId);
}
namespace SmartRetail360.Application.Interfaces.Auth.AccessControl;

public interface IResourceAttributeResolver
{
    Task<Dictionary<string, object>> ResolveAsync(string resourceType, string? resourceId);
}
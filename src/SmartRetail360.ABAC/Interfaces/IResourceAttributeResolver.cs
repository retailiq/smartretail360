namespace SmartRetail360.ABAC.Interfaces;

public interface IResourceAttributeResolver
{
    Task<Dictionary<string, object>> ResolveAsync(string resourceType, string? resourceId);
}
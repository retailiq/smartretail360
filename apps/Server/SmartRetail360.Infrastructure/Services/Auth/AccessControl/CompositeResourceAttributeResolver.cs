using Microsoft.Extensions.Logging;
using SmartRetail360.Application.Interfaces.Auth.AccessControl;

namespace SmartRetail360.Infrastructure.Services.Auth.AccessControl;

public class CompositeResourceAttributeResolver : IResourceAttributeResolver
{
    private readonly IEnumerable<ICustomResourceResolver> _customResolvers;
    private readonly ILogger<CompositeResourceAttributeResolver> _logger;

    public CompositeResourceAttributeResolver(
        IEnumerable<ICustomResourceResolver> customResolvers,
        ILogger<CompositeResourceAttributeResolver> logger)
    {
        _customResolvers = customResolvers;
        _logger = logger;
    }

    public async Task<Dictionary<string, object>> ResolveAsync(string resourceType, string? resourceId)
    {
        try
        {
            var resolver = _customResolvers.FirstOrDefault(r => r.CanResolve(resourceType));
            if (resolver == null)
            {
                _logger.LogWarning("[ABAC] No custom resolver registered for resourceType={ResourceType}", resourceType);
                return new();
            }

            return await resolver.ResolveAsync(resourceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ABAC] Failed to resolve attributes for resourceType={ResourceType}", resourceType);
            return new();
        }
    }
}
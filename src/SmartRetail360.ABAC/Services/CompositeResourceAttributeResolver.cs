using SmartRetail360.ABAC.Interfaces;
using SmartRetail360.Execution;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.ABAC.Services;

public class CompositeResourceAttributeResolver : IResourceAttributeResolver
{
    private readonly IEnumerable<ICustomResourceResolver> _customResolvers;
    private readonly ISafeExecutor _safeExecutor;
    private readonly IGuardChecker _guardChecker;

    public CompositeResourceAttributeResolver(
        IEnumerable<ICustomResourceResolver> customResolvers,
        ISafeExecutor safeExecutor,
        IGuardChecker guardChecker)
    {
        _customResolvers = customResolvers;
        _safeExecutor = safeExecutor;
        _guardChecker = guardChecker;
    }
    
    public async Task<Dictionary<string, object>> ResolveAsync(string resourceType, string? resourceId)
    {
        var resolver = _customResolvers.FirstOrDefault(r => r.CanResolve(resourceType));

        var checkResult = await _guardChecker
            .Check(() => resolver == null,
                LogEventType.GetAbacResolverFailure,
                LogReasons.AbacResolverNotFound,
                ErrorCodes.None)
            .ValidateAsync();

        if (checkResult != null)
            return [];

        var execResult = await _safeExecutor.ExecuteAsync(
            () => resolver!.ResolveAsync(resourceId),
            LogEventType.GetAbacResolverFailure,
            LogReasons.AbacResolverExecutionFailed,
            ErrorCodes.None
        );
        if (!execResult.IsSuccess)
            return [];

        return execResult.IsSuccess ? execResult.Response.Data! : [];
    }
}
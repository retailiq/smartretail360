using Microsoft.EntityFrameworkCore;
using SmartRetail360.ABAC.Interfaces;
using SmartRetail360.Execution;
using SmartRetail360.Persistence.Data;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Enums.AccessControl;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.ABAC.Services.Resolvers;

public class AbacPolicyResourceResolver : ICustomResourceResolver
{
    private readonly AppDbContext _db;
    private readonly ISafeExecutor _safeExecutor;
    private readonly IGuardChecker _guardChecker;

    public AbacPolicyResourceResolver(
        AppDbContext db,
        ISafeExecutor safeExecutor,
        IGuardChecker guardChecker)
    {
        _db = db;
        _safeExecutor = safeExecutor;
        _guardChecker = guardChecker;
    }

    public bool CanResolve(string resourceType)
        => resourceType == DefaultResourceType.AbacPolicy.GetEnumMemberValue();

    public async Task<Dictionary<string, object>> ResolveAsync(string? resourceId)
    {
        var resourceIdCheckResult = await _guardChecker
            .Check(() => string.IsNullOrWhiteSpace(resourceId),
                LogEventType.GetAbacResourceFailure,
                LogReasons.AbacResourceIdMissing,
                ErrorCodes.None)
            .ValidateAsync();

        if (resourceIdCheckResult != null)
            return [];

        var policyResult = await _safeExecutor.ExecuteAsync(() =>
                _db.AbacPolicies
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == Guid.Parse(resourceId!)),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );

        var policy = policyResult.Response.Data;

        if (!policyResult.IsSuccess)
            return [];

        var policyCheckResult = await _guardChecker
            .Check(() => policy == null,
                LogEventType.GetAbacResourceFailure,
                LogReasons.AbacPolicyNotFound,
                ErrorCodes.None)
            .ValidateAsync();
        if (policyCheckResult != null)
            return [];

        return new Dictionary<string, object>
        {
            ["tenant_id"] = policy!.TenantId
        };
    }
}
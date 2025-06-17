using Microsoft.EntityFrameworkCore;
using SmartRetail360.ABAC.Interfaces;
using SmartRetail360.Execution;
using SmartRetail360.Persistence.Data;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Enums.AccessControl;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.ABAC.Services.Resolvers;

public class UserResourceResolver : ICustomResourceResolver
{
    private readonly AppDbContext _db;
    private readonly ISafeExecutor _safeExecutor;
    private readonly IGuardChecker _guardChecker;

    public UserResourceResolver(
        AppDbContext db,
        ISafeExecutor safeExecutor,
        IGuardChecker guardChecker)
    {
        _db = db;
        _safeExecutor = safeExecutor;
        _guardChecker = guardChecker;
    }

    public bool CanResolve(string resourceType)
        => resourceType == DefaultResourceType.User.GetEnumMemberValue();

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

        var tenantUserResult = await _safeExecutor.ExecuteAsync(() =>
                _db.TenantUsers
                    .Include(tu => tu.User)
                    .Include(tu => tu.Tenant)
                    .Include(tu => tu.Role)
                    .Where(tu => tu.UserId == Guid.Parse(resourceId!) && tu.DeletedAt == null)
                    .ToListAsync(),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );

        var tenantUsers = tenantUserResult.Response.Data;

        if (!tenantUserResult.IsSuccess)
            return [];

        var tenantUserCheckResult = await _guardChecker
            .Check(() => tenantUsers!.Count == 0,
                LogEventType.GetAbacResourceFailure,
                LogReasons.AbacTenantUserRecordNotFound,
                ErrorCodes.None)
            .Check(
                () => tenantUsers!.Count == 0 ||
                      tenantUsers.Any(tu => tu.Tenant == null || tu.Role == null || tu.User == null),
                LogEventType.GetAbacResourceFailure,
                LogReasons.AbacUserTenantRoleNotFound,
                ErrorCodes.None)
            .ValidateAsync();

        if (tenantUserCheckResult != null)
            return [];

        return new Dictionary<string, object>
        {
            ["user_id"] = tenantUsers![0].User!.Id,
            ["user_name"] = tenantUsers[0].User!.Name,
            ["user_email"] = tenantUsers[0].User!.Email,
            ["is_user_active"] = tenantUsers[0].User!.IsActive,
            ["user_is_system_account"] = tenantUsers[0].User!.IsSystemAccount,
            ["user_status"] = tenantUsers[0].User!.Status,
            ["is_user_verified"] = tenantUsers[0].User!.IsEmailVerified,
            ["tenant_roles"] = tenantUsers.Select(tu => new Dictionary<string, object>
            {
                ["tenant_id"] = tu.TenantId,
                ["role_id"] = tu.Role!.Id,
                ["role_name"] = tu.Role.Name,
                ["tenant_name"] = tu.Tenant!.Name!
            }).ToList()
        };
    }
}
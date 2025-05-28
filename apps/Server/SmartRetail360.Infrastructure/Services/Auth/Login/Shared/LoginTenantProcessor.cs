using Microsoft.EntityFrameworkCore;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Infrastructure.Services.Auth.Login.Interfaces;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Auth.Login.Shared;

public class LoginTenantProcessor
{
    private readonly ILoginContextBase _ctx;

    public LoginTenantProcessor(ILoginContextBase ctx)
    {
        _ctx = ctx;
    }

    public async Task<ApiResponse<LoginResponse>?> LoadTenantsAndRolesAsync()
    {
        var tenantUsersResult = await _ctx.Dep.SafeExecutor.ExecuteAsync(
            () => _ctx.Dep.Db.TenantUsers
                .Where(tu => tu.UserId == _ctx.User!.Id)
                .Include(tu => tu.Tenant)
                .Include(tu => tu.Role)
                .ToListAsync(),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );
        if (!tenantUsersResult.IsSuccess)
            return tenantUsersResult.ToObjectResponse().To<LoginResponse>();

        _ctx.TenantUsers = tenantUsersResult.Response.Data!;

        var tenantUsersCheckResult = await _ctx.Dep.GuardChecker
            .Check(() => _ctx.TenantUsers.Count == 0,
                LogEventType.UserLoginFailure, LogReasons.TenantUserRecordNotFound,
                ErrorCodes.TenantUserRecordNotFound)
            .ValidateAsync();
        if (tenantUsersCheckResult != null)
            return tenantUsersCheckResult.To<LoginResponse>();

        var inactiveTenants = _ctx.TenantUsers
            .Where(tu => !tu.Tenant!.IsActive)
            .ToList();
        var tenantCount = _ctx.TenantUsers
            .Where(tu => tu.Tenant != null)
            .Select(tu => tu.Tenant!.Id)
            .Distinct()
            .Count();
        var tenantStatusResult = await _ctx.Dep.GuardChecker
            .Check(() => tenantCount == 0,
                LogEventType.UserLoginFailure, LogReasons.TenantNotFound,
                ErrorCodes.TenantNotFound)
            .Check(() => inactiveTenants.Count == _ctx.TenantUsers.Count,
                LogEventType.UserLoginFailure, LogReasons.AllTenantsDisabled,
                ErrorCodes.AllTenantsDisabled)
            .ValidateAsync();
        if (tenantStatusResult != null)
            return tenantStatusResult.To<LoginResponse>();

        var roleIds = _ctx.TenantUsers.Select(tu => tu.RoleId).Distinct().ToList();
        _ctx.Roles = await _ctx.Dep.RedisOperation.GetSystemRolesByIdsAsync(roleIds);
        var roleResult = await _ctx.Dep.GuardChecker
            .Check(() => _ctx.Roles.Count == 0,
                LogEventType.UserLoginFailure, LogReasons.RoleListNotFound,
                ErrorCodes.InternalServerError)
            .ValidateAsync();
        if (roleResult != null)
            return roleResult.To<LoginResponse>();

        return null;
    }
}
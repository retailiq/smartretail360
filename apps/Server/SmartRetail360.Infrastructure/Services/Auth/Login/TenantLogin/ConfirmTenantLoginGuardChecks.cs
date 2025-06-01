using Microsoft.EntityFrameworkCore;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Auth.Login.TenantLogin;

public class ConfirmTenantLoginGuardChecks
{
    private readonly ConfirmTenantLoginContext _ctx;

    public ConfirmTenantLoginGuardChecks(ConfirmTenantLoginContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ApiResponse<ConfirmTenantLoginResponse>?> CheckTenantUserValidityAsync()
    {
        var tenantUserResult = await _ctx._dep.SafeExecutor.ExecuteAsync(
            () => _ctx._dep.Db.TenantUsers
                .Include(tu => tu.Tenant)
                .Include(tu => tu.Role)
                .Include(tu => tu.User)
                .FirstOrDefaultAsync(tu =>
                    tu.UserId == _ctx.Request.UserId &&
                    tu.TenantId == _ctx.Request.TenantId),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );
        if (!tenantUserResult.IsSuccess)
            return tenantUserResult.ToObjectResponse().To<ConfirmTenantLoginResponse>();

        _ctx.TenantUser = tenantUserResult.Response.Data;

        var accountCheckResult = await _ctx._dep.GuardChecker
            .Check(() => _ctx.TenantUser == null,
                LogEventType.ConfirmTenantLoginFailure, LogReasons.TenantUserRecordNotFound,
                ErrorCodes.TenantUserRecordNotFound)
            .Check(() => _ctx.TenantUser is { User: null },
                LogEventType.ConfirmTenantLoginFailure, LogReasons.AccountNotFound,
                ErrorCodes.AccountNotFound)
            .Check(() => _ctx.TenantUser is { Tenant: null },
                LogEventType.ConfirmTenantLoginFailure, LogReasons.TenantNotFound,
                ErrorCodes.TenantNotFound)
            .Check(() => !_ctx.TenantUser!.IsActive,
                LogEventType.ConfirmTenantLoginFailure, LogReasons.TenantUserDisabled,
                ErrorCodes.TenantUserDisabled)
            .Check(() => !_ctx.TenantUser!.Tenant!.IsActive,
                LogEventType.ConfirmTenantLoginFailure, LogReasons.TenantDisabled,
                ErrorCodes.TenantDisabled)
            .ValidateAsync();
        if (accountCheckResult != null)
            return accountCheckResult.To<ConfirmTenantLoginResponse>();

        _ctx._dep.UserContext.Inject(new UserExecutionContext
        {
            RoleId = _ctx.TenantUser!.Role!.Id,
            RoleName = _ctx.TenantUser.Role.Name,
            Email = _ctx.TenantUser.User!.Email,
            UserName = _ctx.TenantUser.User.Name,
            UserId = _ctx.TenantUser.UserId,
            TenantId = _ctx.TenantUser.TenantId
        });

        return null;
    }
}
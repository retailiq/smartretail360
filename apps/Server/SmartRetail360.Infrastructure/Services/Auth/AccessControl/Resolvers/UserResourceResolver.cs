using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartRetail360.Application.Common.UserContext;
using SmartRetail360.Application.Interfaces.Auth.AccessControl;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.Shared.Enums.AccessControl;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Infrastructure.Services.Auth.AccessControl.Resolvers;

public class UserResourceResolver : ICustomResourceResolver
{
    private readonly AppDbContext _db;
    private readonly ILogger<UserResourceResolver> _logger;
    private readonly IUserContextService _userContext;

    public UserResourceResolver(
        AppDbContext db,
        ILogger<UserResourceResolver> logger,
        IUserContextService userContext)
    {
        _db = db;
        _logger = logger;
        _userContext = userContext;
    }

    public bool CanResolve(string resourceType)
        => resourceType == DefaultResourceType.User.GetEnumMemberValue();

    public async Task<Dictionary<string, object>> ResolveAsync(string? resourceId)
    {
        if (string.IsNullOrWhiteSpace(resourceId))
        {
            _logger.LogWarning("[ABAC] Missing resourceId for resourceType=user");
            return new();
        }

        try
        {
            _logger.LogInformation("[ABAC] Resolving user resource with id={ResourceId}", resourceId);
            var tenantUser = await _db.TenantUsers
                .Include(tu => tu.User)
                .Include(tu => tu.Tenant)
                .Include(tu => tu.Role)
                .FirstOrDefaultAsync(tu => tu.UserId == Guid.Parse(resourceId) && tu.DeletedAt == null);

            if (tenantUser == null)
            {
                _logger.LogWarning("[ABAC] TenantUser not found. id={ResourceId}", resourceId);
                return new();
            }

            if (tenantUser.User == null || tenantUser.Tenant == null || tenantUser.Role == null)
            {
                _logger.LogWarning("[ABAC] User/Tenant/Role entity is unexpectedly null. id={ResourceId}", resourceId);
                return new();
            }

            _userContext.Inject(new UserExecutionContext
            {
                UserEntity = tenantUser.User,
                TenantUserEntity = tenantUser,
                TenantEntity = tenantUser.Tenant,
                RoleEntity = tenantUser.Role
            });

            _logger.LogInformation("[ABAC] Resolved user resource: {User}", tenantUser.User);

            return new Dictionary<string, object>
            {
                ["id"] = tenantUser.User.Id,
                ["name"] = tenantUser.User.Name,
                ["email"] = tenantUser.User.Email,
                ["is_user_active"] = tenantUser.User.IsActive,
                ["user_status"] = tenantUser.User.Status,
                ["is_verified"] = tenantUser.User.IsEmailVerified,
                ["tenant_id"] = tenantUser.TenantId,
                ["tenant_name"] = tenantUser.Tenant!.Name!,
                ["role_id"] = tenantUser.Role.Id,
                ["role_name"] = tenantUser.Role.Name,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ABAC] Failed to resolve attributes for user resource. id={ResourceId}", resourceId);
            return new();
        }
    }
}
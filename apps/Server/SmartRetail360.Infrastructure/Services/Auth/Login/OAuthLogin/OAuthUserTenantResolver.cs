using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Auth.Login.OAuthLogin;

public class OAuthUserTenantResolver
{
    private readonly OAuthLoginContext _ctx;

    public OAuthUserTenantResolver(OAuthLoginContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ApiResponse<LoginResponse>?> ResolveUserTenantAsync()
    {
        var userProfile = _ctx.UserProfile!;

        var (user, userError) = await _ctx.Dep.PlatformContext.GetUserByEmailAsync(userProfile.Email!);
        if (userError != null)
            return userError.To<LoginResponse>();

        if (user != null)
        {
            _ctx.User = user;
            _ctx.Dep.UserContext.Inject(new UserExecutionContext
            {
                UserId = user.Id,
                UserName = user.Name,
                Email = user.Email
            });
        }

        if (user == null)
        {
            var role = await _ctx.Dep.RedisOperation.GetSystemRoleAsync(SystemRoleType.Owner);
            var roleCheckResult = await _ctx.Dep.GuardChecker
                .Check(() => role == null,
                    LogEventType.CredentialsLoginFailure, LogReasons.RoleListNotFound,
                    ErrorCodes.InternalServerError)
                .ValidateAsync();
            if (roleCheckResult != null)
                return roleCheckResult.To<LoginResponse>();

            var roleId = role!.Id;
            _ctx.Dep.UserContext.Inject(
                new UserExecutionContext { RoleName = role.Name, RoleId = roleId }
            );

            var newUser = new User
            {
                Email = userProfile.Email!.ToLowerInvariant(),
                Name = userProfile.Name,
                TraceId = _ctx.TraceId,
                LastEmailSentAt = DateTime.UtcNow,
                StatusEnum = AccountStatus.Active,
                AvatarUrl = userProfile.AvatarUrl,
                IsEmailVerified = true,
                Locale = string.IsNullOrWhiteSpace(_ctx.Dep.UserContext.Locale)
                    ? LocaleType.En.GetEnumMemberValue()
                    : _ctx.Dep.UserContext.Locale.ToEnumFromMemberValue<LocaleType>().GetEnumMemberValue(),
            };

            var newTenant = new Tenant
            {
                TraceId = _ctx.TraceId,
                CreatedBy = newUser.Id,
                StatusEnum = AccountStatus.Active,
            };

            var newTenantUser = new TenantUser
            {
                UserId = newUser.Id,
                TenantId = newTenant.Id,
                RoleId = roleId,
                IsActive = true,
                TraceId = _ctx.TraceId,
                CreatedBy = newUser.Id,
            };

            var saveResult = await _ctx.Dep.SafeExecutor.ExecuteAsync(
                async () =>
                {
                    _ctx.Dep.Db.Users.Add(newUser);
                    _ctx.Dep.Db.Tenants.Add(newTenant);
                    _ctx.Dep.Db.TenantUsers.Add(newTenantUser);
                    await _ctx.Dep.Db.SaveChangesAsync();
                    await _ctx.Dep.AbacPolicyService.CreateDefaultPoliciesForTenantAsync(newTenant.Id, true);
                },
                LogEventType.DatabaseError,
                LogReasons.DatabaseSaveFailed,
                ErrorCodes.DatabaseUnavailable
            );
            if (!saveResult.IsSuccess)
                return saveResult.ToObjectResponse().To<LoginResponse>();

            _ctx.User = newUser;
            _ctx.Dep.UserContext.Inject(new UserExecutionContext
            {
                UserId = newUser.Id,
                UserName = newUser.Name,
                Email = newUser.Email,
                TenantId = newTenant.Id
            });
        }

        var (oauthAccount, oauthAccountError) =
            await _ctx.Dep.PlatformContext.GetOAuthAccountAsync(userProfile.Email!, userProfile.Provider);
        if (oauthAccountError != null)
            return oauthAccountError.To<LoginResponse>();

        if (oauthAccount != null)
            return null;

        if (oauthAccount == null)
        {
            var newOAuthAccount = new OAuthAccount
            {
                UserId = _ctx.User!.Id,
                ProviderEnum = userProfile.Provider,
                ProviderUserId = userProfile.ProviderUserId,
                Email = userProfile.Email!.ToLowerInvariant(),
                Name = userProfile.Name,
                AvatarUrl = userProfile.AvatarUrl,
                TraceId = _ctx.TraceId,
            };

            var saveOAuthAccountResult = await _ctx.Dep.SafeExecutor.ExecuteAsync(
                async () =>
                {
                    _ctx.Dep.Db.OAuthAccounts.Add(newOAuthAccount);
                    await _ctx.Dep.Db.SaveChangesAsync();
                },
                LogEventType.DatabaseError,
                LogReasons.DatabaseSaveFailed,
                ErrorCodes.DatabaseUnavailable
            );
            if (!saveOAuthAccountResult.IsSuccess)
                return saveOAuthAccountResult.ToObjectResponse().To<LoginResponse>();
        }

        return null;
    }
}
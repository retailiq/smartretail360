using SmartRetail360.Domain.Entities;
using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Messaging.Payloads;
using SmartRetail360.Shared.Responses;
using UserEntity = SmartRetail360.Domain.Entities.User;

namespace SmartRetail360.Platform.Interfaces;

public interface IPlatformContextService
{
    Task<(UserEntity?, ApiResponse<object>?)> GetUserByIdAsync(Guid userId);
    Task<(UserEntity?, ApiResponse<object>?)> GetUserByEmailAsync(string email);
    Task<(List<TenantUser>?, ApiResponse<object>?)> GetTenantUserByTenantAndUserIdAsync(Guid userId, Guid tenantId);
    Task<(List<TenantUser>?, ApiResponse<object>?)> GetTenantUserByUserIdAsync(Guid userId);
    Task<(Tenant?, ApiResponse<object>?)> GetTenantAsync(Guid tenantId);
    Task<(List<Tenant>?, ApiResponse<object>?)> GetTenantsByIdsAsync(List<Guid> tenantIds);
    Task<(OAuthAccount?, ApiResponse<object>?)> GetOAuthAccountAsync(string email, OAuthProvider provider);
    Task<(RefreshToken?, ApiResponse<object>?)> GetRefreshTokenAsync(string token);
    Task<(List<AbacPolicy>?, ApiResponse<object>?)> GetAbacPoliciesByTenantIdAsync(Guid tenantId);

    Task<ApiResponse<object>?> SendRegistrationInvitationEmailAsync(string token, ActivationEmailPayload payload);

    Task<(AbacPolicy?, ApiResponse<object>?)> GetAbacPolicyByIdAsync(Guid policyId);

    Task<(TenantUser?, ApiResponse<object>?)> GetTenantUserRolesAsync(Guid tenantId, Guid userId);
}
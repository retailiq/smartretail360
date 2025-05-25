using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Messaging.Payloads;
using SmartRetail360.Shared.Responses;
using UserEntity = SmartRetail360.Domain.Entities.User;

namespace SmartRetail360.Application.Interfaces.Common;

public interface IPlatformContextService
{
    Task<(UserEntity?, ApiResponse<object>?)> GetUserByIdAsync(Guid userId);
    Task<(UserEntity?, ApiResponse<object>?)> GetUserByEmailAsync(string email);
    Task<(TenantUser?, ApiResponse<object>?)> GetTenantUserAsync(Guid userId);
    Task<(Tenant?, ApiResponse<object>?)> GetTenantAsync(Guid tenantId);
    
    Task<ApiResponse<object>?> SendRegistrationInvitationEmailAsync(string token, ActivationEmailPayload payload);
}
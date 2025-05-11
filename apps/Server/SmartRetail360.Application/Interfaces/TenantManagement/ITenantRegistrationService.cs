using SmartRetail360.Application.DTOs.Auth.Requests;
using SmartRetail360.Application.DTOs.Auth.Responses;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Application.Interfaces.TenantManagement;

public interface ITenantRegistrationService
{
    Task<ApiResponse<TenantRegisterResponse>> RegisterTenantAsync(TenantRegisterRequest request);
}
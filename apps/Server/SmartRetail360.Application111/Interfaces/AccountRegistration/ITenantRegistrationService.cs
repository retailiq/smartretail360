using SmartRetail360.Application.DTOs.AccountRegistration.Requests;
using SmartRetail360.Application.DTOs.AccountRegistration.Responses;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Application.Interfaces.AccountRegistration;

public interface ITenantRegistrationService
{
    Task<ApiResponse<TenantRegisterResponse>> RegisterTenantAsync(TenantRegisterRequest request);
}
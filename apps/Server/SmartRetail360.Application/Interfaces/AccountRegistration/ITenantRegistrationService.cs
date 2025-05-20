using SmartRetail360.Contracts.AccountRegistration.Requests;
using SmartRetail360.Contracts.AccountRegistration.Responses;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Application.Interfaces.AccountRegistration;

public interface ITenantRegistrationService
{
    Task<ApiResponse<TenantRegisterResponse>> RegisterTenantAsync(TenantRegisterRequest request);
}
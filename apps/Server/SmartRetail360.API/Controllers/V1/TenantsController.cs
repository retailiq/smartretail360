using Microsoft.AspNetCore.Mvc;
using SmartRetail360.Application.DTOs.Auth.Requests;
using SmartRetail360.Application.DTOs.Auth.Responses;
using SmartRetail360.Application.Interfaces.TenantManagement;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/tenants")]
public class TenantsController : ControllerBase
{
    private readonly ITenantRegistrationService _tenantRegistrationService;

    public TenantsController(ITenantRegistrationService tenantRegistrationService)
    {
        _tenantRegistrationService = tenantRegistrationService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TenantRegisterResponse>>> RegisterTenant(TenantRegisterRequest request)
    {
        var result = await _tenantRegistrationService.RegisterTenantAsync(request);
        return StatusCode(201, result);
    }
}
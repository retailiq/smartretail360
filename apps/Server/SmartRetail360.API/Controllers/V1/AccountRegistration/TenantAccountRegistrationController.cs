using Microsoft.AspNetCore.Mvc;
using SmartRetail360.Application.DTOs.AccountRegistration.Requests;
using SmartRetail360.Application.DTOs.AccountRegistration.Responses;
using SmartRetail360.Application.Interfaces.AccountRegistration;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.API.Controllers.V1.AccountRegistration;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/tenants")]
public class TenantAccountRegistrationController : ControllerBase
{
    private readonly ITenantRegistrationService _tenantRegistrationService;
    
    public TenantAccountRegistrationController(ITenantRegistrationService tenantRegistrationService)
    {
        _tenantRegistrationService = tenantRegistrationService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TenantRegisterResponse>>> TenantRegister([FromBody] TenantRegisterRequest request)
    {
        var result = await _tenantRegistrationService.RegisterTenantAsync(request);
        return StatusCode(201, result);
    }
}
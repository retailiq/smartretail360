using Microsoft.AspNetCore.Mvc;
using SmartRetail360.Contracts.AccountRegistration.Requests;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.API.Controllers.V2;

[ApiController]
[ApiVersion("2.0")]
[ApiExplorerSettings(GroupName = "v2")]
[Route("api/v{version:apiVersion}/tenants")]
public class TenantRegistration1Controller : ControllerBase
{
    // private readonly IRegistrationService _registrationService;
    //
    // public TenantRegistrationController(IRegistrationService registrationService)
    // {
    //     _registrationService = registrationService;
    // }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> RegisterTenant1(AccountRegisterRequest request)
    {
        return StatusCode(201, "Tenant registered successfully");
    }
}
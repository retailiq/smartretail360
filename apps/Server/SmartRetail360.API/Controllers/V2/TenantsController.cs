// using Microsoft.AspNetCore.Mvc;
// using SmartRetail360.Application.DTOs.Auth.Requests;
// using SmartRetail360.Application.Interfaces.Auth;
// using SmartRetail360.Shared.Responses;
//
// namespace SmartRetail360.API.Controllers.V2;
//
// [ApiController]
// [ApiVersion("2.0")]
// [Route("api/v{version:apiVersion}/tenants")]
// public class TenantRegistrationController : ControllerBase
// {
//     private readonly IRegistrationService _registrationService;
//
//     public TenantRegistrationController(IRegistrationService registrationService)
//     {
//         _registrationService = registrationService;
//     }
//
//     [HttpPost]
//     public async Task<ActionResult<ApiResponse<object>>> RegisterTenant(TenantRegisterRequest request)
//     {
//         var result = await _registrationService.RegisterTenantV2Async(request);
//         return StatusCode(201, result);
//     }
// }
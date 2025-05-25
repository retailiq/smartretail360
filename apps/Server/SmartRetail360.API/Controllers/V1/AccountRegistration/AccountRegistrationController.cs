using Microsoft.AspNetCore.Mvc;
using SmartRetail360.Application.Interfaces.AccountRegistration;
using SmartRetail360.Contracts.AccountRegistration.Requests;
using SmartRetail360.Contracts.AccountRegistration.Responses;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.API.Controllers.V1.AccountRegistration;

[ApiController]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v{version:apiVersion}/users")]
public class AccountRegistrationController : ControllerBase
{
    private readonly IAccountRegistrationService _accountRegistration;

    public AccountRegistrationController(
        IAccountRegistrationService accountRegistration
    )
    {
        _accountRegistration = accountRegistration;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AccountRegisterResponse>>> RegisterUser(
        [FromBody] AccountRegisterRequest request)
    {
        var result = await _accountRegistration.RegisterUserAsync(request);
        return StatusCode(201, result);
    }
}
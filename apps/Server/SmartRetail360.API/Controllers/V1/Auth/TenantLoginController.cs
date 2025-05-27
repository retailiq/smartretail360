using Microsoft.AspNetCore.Mvc;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.API.Controllers.V1.Auth;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
[ApiExplorerSettings(GroupName = "v1")]
public class TenantLoginController : ControllerBase
{
    private readonly IConfirmTenantLoginService _confirmTenantLogin;

    public TenantLoginController(
        IConfirmTenantLoginService confirmTenantLogin)
    {
        _confirmTenantLogin = confirmTenantLogin;
    }

    [HttpPost("login/tenant")]
    public async Task<ActionResult<ApiResponse<ConfirmTenantLoginResponse>>> ConfirmTenantLogin(
        [FromBody] ConfirmTenantLoginRequest request)
    {
        var result = await _confirmTenantLogin.ConfirmTenantLoginAsync(request);
        return StatusCode(200, result);
    }
}
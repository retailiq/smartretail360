using Microsoft.AspNetCore.Mvc;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Shared.Extensions;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.API.Controllers.V1.Auth;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
[ApiExplorerSettings(GroupName = "v1")]
public class CredentialsLoginController : ControllerBase
{
    private readonly ILoginService _login;

    public CredentialsLoginController(
        ILoginService login)
    {
        _login = login;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _login.LoginAsync(request);
        return result.ToHttpResult();
    }
}
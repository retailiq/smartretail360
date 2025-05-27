using Microsoft.AspNetCore.Mvc;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.API.Controllers.V1.Auth;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth/oauth")]
[ApiExplorerSettings(GroupName = "v1")]
public class OAuthLoginController : ControllerBase
{
    private readonly IOAuthLoginService _oauthLogin;

    public OAuthLoginController(
        IOAuthLoginService oauthLogin)
    {
        _oauthLogin = oauthLogin;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> OAuthLogin([FromBody] OAuthLoginRequest request)
    {
        var result = await _oauthLogin.OAuthLoginAsync(request);
        return StatusCode(200, result);
    }
}
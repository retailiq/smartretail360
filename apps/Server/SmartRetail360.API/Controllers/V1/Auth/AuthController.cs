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
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("validate-token")]
    public async Task<IActionResult> ValidateToken()
    {
        var result = await _authService.ValidateToken();
        return result.ToHttpResult();
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshAsync(request);
        return result.ToHttpResult();
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var result = await _authService.LogoutAsync();
        return result.ToHttpResult();
    }
}
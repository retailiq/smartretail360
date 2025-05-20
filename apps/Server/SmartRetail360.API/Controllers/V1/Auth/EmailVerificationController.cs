using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Application.Interfaces.Auth.Configuration;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.API.Controllers.V1.Auth;

[ApiController]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v{version:apiVersion}/auth/emails")]
public class EmailVerificationController : ControllerBase
{
    private readonly IEmailVerificationDispatchService _emailVerificationDispatchService;
    
    public EmailVerificationController(
        IEmailVerificationDispatchService emailVerificationDispatchService
        )
    {
        _emailVerificationDispatchService = emailVerificationDispatchService;
    }

    [HttpPost("verify")]
    public async Task<ActionResult<ApiResponse<object>>> VerifyEmail([FromBody] EmailVerificationRequest request)
    {
        var result = await _emailVerificationDispatchService.DispatchAsync(request.Type, request.Token);
        return StatusCode(200, result);
    }
}
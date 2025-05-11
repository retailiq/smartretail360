using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.DTOs.Auth.Requests;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.API.Controllers.V1.Auth;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth/email")]
public class EmailVerificationController : ControllerBase
{
    private readonly IEmailVerificationService _emailVerificationService;
    

    public EmailVerificationController(IEmailVerificationService emailVerificationService)
    {
        _emailVerificationService = emailVerificationService;
    }

    [HttpGet("verify")]
    public async Task<ActionResult<ApiResponse<object>>> VerifyEmail([FromQuery] EmailVerificationQuery query)
    {
        var result = await _emailVerificationService.VerifyEmailAsync(query.Token);
        return StatusCode(200, result);
    }
}
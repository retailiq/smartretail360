using Microsoft.AspNetCore.Mvc;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Shared.Extensions;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.API.Controllers.V1.Auth;

[ApiController]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v{version:apiVersion}/auth/emails/")]
public class EmailVerificationController : ControllerBase
{
    private readonly IAccountEmailVerificationService _accountActivationEmailVerification;

    public EmailVerificationController(
        IAccountEmailVerificationService accountActivationEmailVerification)
    {
        _accountActivationEmailVerification = accountActivationEmailVerification;
    }


    [HttpPost("verify")]
    public async Task<IActionResult> VerifyAccountActivationEmail([FromBody] EmailVerificationRequest request)
    {
        var result = await _accountActivationEmailVerification.VerifyEmailAsync(request.Token);
        return result.ToHttpResult();
    }
}
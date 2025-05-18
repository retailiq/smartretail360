using Microsoft.AspNetCore.Mvc;
using Serilog;
using SmartRetail360.Application.DTOs.Notification;
using SmartRetail360.Application.Interfaces.Notifications.Configuration;
using SmartRetail360.Infrastructure.Services.Notifications.Configuration;

namespace SmartRetail360.API.Controllers.Internal;

[ApiController]
[Route("api/internal/emails")]
public class InternalEmailController : ControllerBase
{
    private readonly EmailContext _emailContext;

    public InternalEmailController(EmailContext emailContext)
    {
        _emailContext = emailContext;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request)
    {
        Log.Information(
            "📨 Received SendEmail request. Template: {Template}, Email: {Email}, Variables: {@Variables}",
            request.Template,
            request.Email,
            request.Variables
        );
        await _emailContext.SendAsync(
            request.Template,
            request.Email,
            request.Variables ?? new Dictionary<string, string>()
        );

        return Ok(new { success = true });
    }
}
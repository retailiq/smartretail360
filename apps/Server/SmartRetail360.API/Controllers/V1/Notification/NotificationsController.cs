using Microsoft.AspNetCore.Mvc;
using SmartRetail360.Application.Interfaces.Notifications;
using SmartRetail360.Contracts.Notification.Requests;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.API.Controllers.V1.Notification;

[ApiController]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v{version:apiVersion}/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IEmailDispatchService _dispatcher;

    public NotificationsController(IEmailDispatchService dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost("send-email")]
    public async Task<IActionResult> SendEmail([FromBody] EmailNotificationRequest notificationRequest)
    {
        var result = await _dispatcher.DispatchAsync(notificationRequest.Template, notificationRequest.Email);
        return result.ToHttpResult();
    }
}
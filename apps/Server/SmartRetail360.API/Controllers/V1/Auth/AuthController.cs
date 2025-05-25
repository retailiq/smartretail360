using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Application.Interfaces.Notifications;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.API.Controllers.V1.Auth;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
[ApiExplorerSettings(GroupName = "v1")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    // private readonly IEmailNotificationService _emailNotificationService;

    // public AuthController(AppDbContext dbContext, IEmailNotificationService emailNotificationService)
    // {
    //     _dbContext = dbContext;
    //     _emailNotificationService = emailNotificationService;
    // }
}
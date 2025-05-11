using System.Net;
using Microsoft.AspNetCore.Mvc;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Exceptions;
using SmartRetail360.Shared.Options;
using StatusCodes = Microsoft.AspNetCore.Http.StatusCodes;

namespace SmartRetail360.API.Controllers.Dev;

[ApiController]
[Route("api/[Controller]")]
public class DevController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly AppOptions _appOptions;

    public DevController(AppDbContext dbContext, AppOptions appOptions)
    {
        _dbContext = dbContext;
        _appOptions = appOptions;
    }
    
    [HttpGet("test-exception")]
    public IActionResult TestException()
    {
        // throw new CommonException(ErrorCodes.AccountAlreadyActivated, HttpStatusCode.AlreadyReported);
        throw new SecurityException(ErrorCodes.AccountLocked, HttpStatusCode.Forbidden);
    }
}
using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Exceptions;
using SmartRetail360.Shared.Options;
using Serilog;
using Sentry;

namespace SmartRetail360.API.Controllers.Dev;

[ApiController]
[Route("api/[Controller]")]
public class DevController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly AppOptions _appOptions;
    
    private static readonly ActivitySource ActivitySource = new("SmartRetail360.API");

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
    
    [HttpGet("test-sentry")]
    public IActionResult TestSentry()
    {
        SentrySdk.CaptureMessage("Hello Sentry from SmartRetail360");
        return Ok("Sentry message sent.");
    }
    
    [HttpGet("test-serilog")]
    public IActionResult TestSerilog()
    {
        Log.Warning("🔥 Test warning to Loki from SmartRetail360 @ {Time}", DateTime.UtcNow);
        return Ok("Log sent!");
    }
    
    [HttpGet("test-trace")]
    public async Task<IActionResult> TraceTest()
    {
        var traceId = ActivityTraceId.CreateRandom();
        var spanId = ActivitySpanId.CreateRandom();

        var context = new ActivityContext(
            traceId: traceId,
            spanId: spanId,
            traceFlags: ActivityTraceFlags.Recorded);

        var tags = new ActivityTagsCollection
        {
            { "env", "test" },
            { "custom.trace_id", traceId.ToHexString() },
            { "custom.span_id", spanId.ToHexString() }
        };

        using var activity = ActivitySource.StartActivity(
            "TestTraceSpan",
            ActivityKind.Server,
            context,
            tags);

        Log.Information("🚀 Custom trace sent: traceId={TraceId}, spanId={SpanId}", traceId, spanId);

        await Task.Delay(200);

        return Ok($"✅ Trace sent, trace_id = {traceId}");
    }
}
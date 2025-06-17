using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Exceptions;
using SmartRetail360.Shared.Options;
using Serilog;
using SmartRetail360.ABAC.Common;
using SmartRetail360.Persistence;
using SmartRetail360.Persistence.Data;
using SmartRetail360.Shared.Enums.AccessControl;

namespace SmartRetail360.API.Controllers.Dev;

[ApiController]
[Route("api/[Controller]")]
public class DevController : ControllerBase
{
    private static readonly ActivitySource ActivitySource = new("SmartRetail360.API");

    public DevController()
    {
    }

    [HttpGet("debug/protocol")]
    public IActionResult GetProtocol()
    {
        var protocol = HttpContext.Request.Protocol;
        return Ok(new { protocol });
    }

    [HttpGet("headers")]
    public IActionResult GetHeaders()
    {
        var headers = Request.Headers.ToDictionary(
            h => h.Key,
            h => h.Value.ToString()
        );
        return Ok(headers);
    }

    [HttpGet("user/{id}/view")]
    public IActionResult GetUserById(Guid id)
    {
        return Ok($"✅ ABAC allowed: you can view user {id}");
    }

    [HttpGet("user/{id}/edit")]
    public IActionResult EditUserById(Guid id)
    {
        return Ok($"✅ ABAC allowed: you can edit user {id}");
    }

    [HttpGet("user/{id}/delete")]
    public IActionResult DeleteUserById(Guid id)
    {
        return Ok($"✅ ABAC allowed: you can delete user {id}");
    }

    [HttpGet("user/{id}/remove")]
    public IActionResult RemoveUserById(Guid id)
    {
        return Ok($"✅ ABAC allowed: you can remove user {id}");
    }

    [HttpGet("user/{id}/leave")]
    public IActionResult LeaveUserById(Guid id)
    {
        return Ok($"✅ ABAC allowed: you can leave from tenant of user {id}");
    }

    [HttpGet("test-exception")]
    public IActionResult TestException()
    {
        throw new SecurityException(ErrorCodes.AccountLocked);
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
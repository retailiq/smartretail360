using Microsoft.AspNetCore.Mvc;

namespace SmartRetail360.API.Controllers;

/// <summary>
/// System health check endpoint
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Basic liveness check
    /// </summary>
    [HttpGet("live")]
    public IActionResult Liveness()
    {
        return Ok(new { status = "Live" });
    }

    /// <summary>
    /// Readiness check (for DB, Redis, etc. — mock here)
    /// </summary>
    [HttpGet("ready")]
    public IActionResult Readiness()
    {
        // TODO: Add real service checks (e.g., database, cache)
        return Ok(new { status = "Ready" });
    }
}
namespace SmartRetail360.Shared.Utils;

public static class TraceIdGenerator
{
    public static string Generate(string prefix = "trace", string? tenantSlug = null)
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var tenantPart = string.IsNullOrWhiteSpace(tenantSlug) ? "unknown" : tenantSlug;
        var uniqueId = Guid.NewGuid().ToString("N");

        return $"{prefix}-{datePart}-{tenantPart}-{uniqueId}";
    }
}
namespace SmartRetail360.ABAC.Configs;

public class AbacRouteRule
{
    public string Pattern { get; set; } = default!;
    public string Resource { get; set; } = default!;
    public string Action { get; set; } = default!;
}
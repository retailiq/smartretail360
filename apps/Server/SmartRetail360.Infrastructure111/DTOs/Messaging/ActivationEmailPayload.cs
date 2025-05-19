namespace SmartRetail360.Infrastructure.DTOs.Messaging;

public class ActivationEmailPayload
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public string Locale { get; set; } = "en";
    public string Timestamp { get; set; } = DateTime.UtcNow.ToString("o");
    public string? TargetWorkerId { get; set; }
}
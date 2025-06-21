namespace SmartRetail360.Shared.Messaging.Payloads;

public class ActivationEmailPayload
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public string Locale { get; set; } = "en";
    public string Timestamp { get; set; } = DateTime.UtcNow.ToString("o");
    public Guid? UserId { get; set; }
    public Guid? RoleId { get; set; }
    public string? Module { get; set; }
    public string? IpAddress { get; set; }
    public required string Action { get; set; }
    public string? RoleName { get; set; }
    public string? LogId { get; set; }
    public required string EmailTemplate { get; set; }
    public required string UserName { get; set; }
    public string EmailValidationMinutes { get; set; } = string.Empty;
}
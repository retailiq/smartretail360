namespace SmartRetail360.Application.Models;

public class RefreshTokenCreationContext
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public Guid RoleId { get; set; }
    public string IpAddress { get; set; } = default!;
    public int ExpiryDays { get; set; }
    public string Email { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Locale { get; set; } = default!;
    public string TraceId { get; set; } = default!;
}
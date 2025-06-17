namespace SmartRetail360.Application.Models;

public class AccessTokenCreationContext
{
    public string UserId { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string TenantId { get; set; } = default!;
    public string RoleId { get; set; } = default!;
    public string RoleName { get; set; } = default!;
    public string TraceId { get; set; } = default!;
    public string Environment { get; set; } = default!;
}
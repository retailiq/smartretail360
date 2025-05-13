using System.ComponentModel.DataAnnotations.Schema;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Domain.Entities;

public class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; } = null;
    public string Slug { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public string? Industry { get; set; } = string.Empty;
    public int? Size { get; set; } = null;
    public string? LogoUrl { get; set; } = string.Empty;
    public string Status { get; set; } = StringCaseConverter.ToSnakeCase(nameof(TenantStatus.PendingVerification));
    [NotMapped]
    public TenantStatus StatusEnum
    {
        get => Enum.Parse<TenantStatus>(StringCaseConverter.ToPascalCase(Status));
        set => Status = StringCaseConverter.ToSnakeCase(value.ToString());
    }
    public string Plan { get; set; } = StringCaseConverter.ToSnakeCase(nameof(AccountPlan.Free));
    [NotMapped]
    public AccountPlan PlanEnum
    {
        get => Enum.Parse<AccountPlan>(StringCaseConverter.ToPascalCase(Plan));
        set => Plan = StringCaseConverter.ToSnakeCase(value.ToString());
    }
    public string? EmailVerificationToken { get; set; } = null;
    public bool IsEmailVerified { get; set; } = false;
    public bool IsFirstLogin { get; set; } = true;
    public string TraceId { get; set; } = string.Empty;
    public Guid? LastUpdatedBy { get; set; }  = null;
    public DateTime? LastEmailSentAt { get; set; } = null;
    public DateTime? LastLoginAt { get; set; } = null;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; } = null;
}
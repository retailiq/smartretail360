using System.ComponentModel.DataAnnotations.Schema;
using SmartRetail360.Domain.Interfaces;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Domain.Entities;

public class Tenant : IHasCreatedAt, IHasUpdatedAt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; } = null;
    public string? Slug { get; set; } = null;
    public string? Industry { get; set; } = string.Empty;
    public int? Size { get; set; } = null;
    public string? LogoUrl { get; set; } = string.Empty;
    public string Status { get; set; } = StringCaseConverter.ToSnakeCase(nameof(AccountStatus.PendingVerification));
    [NotMapped]
    public AccountStatus StatusEnum
    {
        get => Enum.Parse<AccountStatus>(StringCaseConverter.ToPascalCase(Status));
        set => Status = StringCaseConverter.ToSnakeCase(value.ToString());
    }
    public string Plan { get; set; } = StringCaseConverter.ToSnakeCase(nameof(AccountPlan.Free));
    [NotMapped]
    public AccountPlan PlanEnum
    {
        get => Enum.Parse<AccountPlan>(StringCaseConverter.ToPascalCase(Plan));
        set => Plan = StringCaseConverter.ToSnakeCase(value.ToString());
    }
    public string TraceId { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; }
    public Guid? LastUpdatedBy { get; set; }  = null;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; } = null;
    public bool IsActive { get; set; } = true;
    public DateTime? DeactivatedAt { get; set; } = null;
    public Guid? DeactivatedBy { get; set; } = null;
    public string DeactivationReason { get; set; } = StringCaseConverter.ToSnakeCase(nameof(AccountBanReason.None));
    [NotMapped]
    public AccountBanReason DeactivationReasonEnum
    {
        get => Enum.Parse<AccountBanReason>(StringCaseConverter.ToPascalCase(DeactivationReason));
        set => DeactivationReason = StringCaseConverter.ToSnakeCase(value.ToString());
    }
}
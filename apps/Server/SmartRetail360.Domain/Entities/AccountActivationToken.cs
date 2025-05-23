using System.ComponentModel.DataAnnotations.Schema;
using SmartRetail360.Domain.Interfaces;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Domain.Entities;

public class AccountActivationToken : IHasCreatedAt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Status { get; set; } = StringCaseConverter.ToSnakeCase(nameof(ActivationTokenStatus.Pending));
    [NotMapped]
    public ActivationTokenStatus StatusEnum
    {
        get => Enum.Parse<ActivationTokenStatus>(StringCaseConverter.ToPascalCase(Status));
        set => Status = StringCaseConverter.ToSnakeCase(value.ToString());
    }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string TraceId { get; set; } = string.Empty;
}
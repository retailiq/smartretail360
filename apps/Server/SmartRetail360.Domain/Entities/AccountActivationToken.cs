using System.ComponentModel.DataAnnotations.Schema;
using SmartRetail360.Domain.Interfaces;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Domain.Entities;

public class AccountActivationToken : IHasCreatedAt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Status { get; set; } = ActivationTokenStatus.Pending.GetEnumMemberValue();
    [NotMapped]
    public ActivationTokenStatus StatusEnum
    {
        get => Status.ToEnumFromMemberValue<ActivationTokenStatus>();
        set => Status = value.GetEnumMemberValue();
    }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string TraceId { get; set; } = string.Empty;
    public string Source { get; set; } = ActivationSource.None.GetEnumMemberValue();
    [NotMapped]
    public ActivationSource SourceEnum
    {
        get => Source.ToEnumFromMemberValue<ActivationSource>();
        set => Source = value.GetEnumMemberValue();
    }
}
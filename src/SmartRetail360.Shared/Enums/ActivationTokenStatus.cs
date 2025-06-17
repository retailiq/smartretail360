using System.Runtime.Serialization;

namespace SmartRetail360.Shared.Enums;

public enum ActivationTokenStatus
{
    [EnumMember(Value = "pending")]
    Pending = 0,

    [EnumMember(Value = "used")]
    Used = 1,

    [EnumMember(Value = "expired")]
    Expired = 2,

    [EnumMember(Value = "revoked")]
    Revoked = 3
}
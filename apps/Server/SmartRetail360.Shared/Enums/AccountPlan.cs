using System.Runtime.Serialization;

namespace SmartRetail360.Shared.Enums;

public enum AccountPlan
{
    [EnumMember(Value = "free")]
    Free,

    [EnumMember(Value = "starter")]
    Starter,

    [EnumMember(Value = "pro")]
    Pro,

    [EnumMember(Value = "enterprise")]
    Enterprise
}
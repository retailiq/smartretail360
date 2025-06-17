using System.Runtime.Serialization;

namespace SmartRetail360.Shared.Enums;

public enum ActivationSource
{
    [EnumMember(Value = "none")]
    None,

    [EnumMember(Value = "registration")]
    Registration,

    [EnumMember(Value = "invitation")]
    Invitation
}
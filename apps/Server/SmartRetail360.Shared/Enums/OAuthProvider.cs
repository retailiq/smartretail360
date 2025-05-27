using System.Runtime.Serialization;

namespace SmartRetail360.Shared.Enums;

public enum OAuthProvider
{
    [EnumMember(Value = "none")]
    None,

    [EnumMember(Value = "google")]
    Google,

    [EnumMember(Value = "apple")]
    Apple,

    [EnumMember(Value = "facebook")]
    Facebook,

    [EnumMember(Value = "microsoft")]
    Microsoft
}
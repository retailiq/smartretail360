using System.Runtime.Serialization;

namespace SmartRetail360.Shared.Enums;

public enum SystemRoleType
{
    [EnumMember(Value = "owner")]
    Owner,

    [EnumMember(Value = "admin")]
    Admin,

    [EnumMember(Value = "analyst")]
    Analyst,

    [EnumMember(Value = "member")]
    Member,

    [EnumMember(Value = "guest")]
    Guest
}
using System.Runtime.Serialization;

namespace SmartRetail360.Shared.Enums;

public enum AccountStatus
{
    [EnumMember(Value = "pending_verification")]
    PendingVerification, // Awaiting email verification

    [EnumMember(Value = "active")]
    Active, // Fully verified and active

    [EnumMember(Value = "suspended")]
    Suspended, // Temporarily suspended (e.g., under review or inactive)

    [EnumMember(Value = "banned")]
    Banned, // Permanently banned due to violations

    [EnumMember(Value = "locked")]
    Locked, // Locked due to suspicious activity or security risk

    [EnumMember(Value = "deleted")]
    Deleted // Deleted by user or admin (soft delete)
}
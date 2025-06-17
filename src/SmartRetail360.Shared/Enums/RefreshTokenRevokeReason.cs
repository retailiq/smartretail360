using System.Runtime.Serialization;

namespace SmartRetail360.Shared.Enums;

public enum RefreshTokenRevokeReason
{
    [EnumMember(Value = "none")]
    None,

    [EnumMember(Value = "manual_revocation")]
    ManualRevocation,

    [EnumMember(Value = "logout")]
    Logout,

    [EnumMember(Value = "expired")]
    Expired,

    [EnumMember(Value = "token_rotation")]
    TokenRotation,

    [EnumMember(Value = "compromised")]
    Compromised,

    [EnumMember(Value = "password_changed")]
    PasswordChanged,

    [EnumMember(Value = "account_locked")]
    AccountLocked,

    [EnumMember(Value = "device_removal")]
    DeviceRemoval
}
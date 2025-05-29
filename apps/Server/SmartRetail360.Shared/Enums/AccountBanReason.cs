using System.Runtime.Serialization;

namespace SmartRetail360.Shared.Enums;

public enum AccountBanReason
{
    [EnumMember(Value = "none")]
    None = 0,

    // ✅ General reasons
    [EnumMember(Value = "manual")]
    ManualDeactivation,         // Manually deactivated by admin or operator

    [EnumMember(Value = "terms_violation")]
    ViolationOfTerms,           // Violation of terms of service (e.g., abuse, attack attempts)

    [EnumMember(Value = "security_risk")]
    SecurityRisk,               // Security risk detected (e.g., account compromise, suspicious access)

    [EnumMember(Value = "inactive")]
    InactiveTooLong,            // Prolonged inactivity without login or usage

    // ✅ User-related reasons
    [EnumMember(Value = "invalid_email")]
    InvalidEmailOrUnverified,   // Invalid or unverified email address

    [EnumMember(Value = "duplicate")]
    DuplicateAccountDetected,   // Duplicate account detected (e.g., same email or device)

    [EnumMember(Value = "user_requested")]
    ManualRequestByUser,         // Deactivation requested by the user
    
    [EnumMember(Value = "login_failure_limit")]
    LoginFailureLimit // Too many failed login attempts
}
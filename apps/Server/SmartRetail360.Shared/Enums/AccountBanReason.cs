namespace SmartRetail360.Shared.Enums;

public enum AccountBanReason
{
    None = 0,

    // Common reasons
    ManualDeactivation,         // Manually deactivated (by admin/operator)
    ViolationOfTerms,           // Violation of terms of service
    SecurityRisk,               // Security risk detected (e.g., account compromise)
    SuspiciousBehavior,         // Suspicious behavior (e.g., API abuse, attack attempts)
    AbuseOrSpam,                // System abuse or spam activities
    InactiveTooLong,            // Prolonged inactivity

    // User-specific reasons
    InvalidEmailOrUnverified,   // Invalid or unverified email address
    DuplicateAccountDetected,   // Duplicate account detected
    ManualRequestByUser,        // Deactivation requested by the user

    // Tenant-specific reasons
    SubscriptionExpired,        // Subscription expired without renewal
    PaymentFailure,             // Billing/payment failure
    ExceededQuotaLimit,         // Quota limit exceeded
    OrganizationShutdown,       // Organization closed or merged
    UnderInvestigation,         // Under compliance or security investigation
}
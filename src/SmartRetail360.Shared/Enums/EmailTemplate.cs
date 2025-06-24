using System.Runtime.Serialization;

namespace SmartRetail360.Shared.Enums;

public enum EmailTemplate
{
    [EnumMember(Value = "userRegistrationActivation")]
    UserRegistrationActivation,

    [EnumMember(Value = "userInvitationActivation")]
    UserInvitationActivation,

    [EnumMember(Value = "verificationCode")]
    VerificationCode,

    [EnumMember(Value = "passwordReset")]
    PasswordReset,

    [EnumMember(Value = "marketing")]
    Marketing,
    
    [EnumMember(Value = "userEmailUpdate")]
    UseEmailUpdate
}
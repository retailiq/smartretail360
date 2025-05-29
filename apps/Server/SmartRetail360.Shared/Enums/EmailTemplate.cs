using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SmartRetail360.Shared.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
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
    Marketing
}
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Shared.Utils;

public static class TokenHelper
{
    public static string GenerateActivateAccountToken()
    {
        return Guid.NewGuid().ToString("N");
    }

    public static string GetLogAction(ActivationSource source)
    {
        return source == ActivationSource.Registration
            ? LogActions.UserRegistrationActivationEmailResend
            : LogActions.UserInvitationActivationEmailResend;
    }
}
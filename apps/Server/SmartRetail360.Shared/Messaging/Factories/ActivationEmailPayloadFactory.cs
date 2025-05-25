using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Messaging.Payloads;

namespace SmartRetail360.Shared.Messaging.Factories;

public static class ActivationEmailPayloadFactory
{
    public static ActivationEmailPayload Create(
        string email,
        string userName,
        string token,
        string action,
        EmailTemplate template,
        int minutes)
    {
        return new ActivationEmailPayload
        {
            Email = email,
            Token = token,
            Timestamp = DateTime.UtcNow.ToString("o"),
            Action = action,
            EmailTemplate = template,
            UserName = userName,
            EmailValidationMinutes = minutes.ToString()
        };
    }
}
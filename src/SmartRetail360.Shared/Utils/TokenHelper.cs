using System.Security.Cryptography;
using System.Text;
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

    public static string GenerateRefreshToken(int byteLength = 64)
    {
        var randomBytes = new byte[byteLength];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }

    public static string HashToken(string token)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
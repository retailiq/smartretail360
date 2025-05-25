using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Options;

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
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "");
    }
}
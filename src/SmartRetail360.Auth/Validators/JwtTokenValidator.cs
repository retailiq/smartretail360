using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SmartRetail360.Auth.Validators;

public static class JwtTokenValidator
{
    public static bool TryValidateToken(
        string token,
        string jwtSecret,
        out ClaimsPrincipal? principal,
        out SecurityToken? validatedToken,
        out Exception? exception)
    {
        var handler = new JwtSecurityTokenHandler();
        try
        {
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            principal = handler.ValidateToken(token, parameters, out validatedToken);
            exception = null;
            return true;
        }
        catch (Exception ex)
        {
            principal = null;
            validatedToken = null;
            exception = ex;
            return false;
        }
    }
}
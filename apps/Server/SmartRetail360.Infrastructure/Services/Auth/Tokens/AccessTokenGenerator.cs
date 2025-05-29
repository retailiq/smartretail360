using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Shared.Options;

namespace SmartRetail360.Infrastructure.Services.Auth.Tokens;

public class AccessTokenGenerator : IAccessTokenGenerator
{
    private readonly AppOptions _options;

    public AccessTokenGenerator(IOptions<AppOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateToken(
        string userId,
        string email,
        string name,
        string tenantId,
        string roleId,
        string locale,
        string traceId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.JwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;
        var expires = now.AddSeconds(_options.AccessTokenExpirySeconds);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("email", email),
            new Claim("name", name),
            new Claim("tenant_id", tenantId),
            new Claim("role_id", roleId),
            new Claim("locale", locale),
            new Claim("trace_id", traceId),
            new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: "SmartRetail360",
            audience: "SmartRetail360Client",
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
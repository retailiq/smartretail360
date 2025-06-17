using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Application.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Options;

namespace SmartRetail360.Infrastructure.Services.Auth.Tokens;

public class AccessTokenGenerator : IAccessTokenGenerator
{
    private readonly AppOptions _options;

    public AccessTokenGenerator(IOptions<AppOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateToken(AccessTokenCreationContext payload)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.JwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;
        var expires = now.AddSeconds(_options.AccessTokenExpirySeconds);

        var claims = new[]
        {
            new Claim("user_id", payload.UserId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("user_email", payload.Email),
            new Claim("user_name", payload.UserName),
            new Claim("tenant_id", payload.TenantId),
            new Claim("role_id", payload.RoleId),
            new Claim("trace_id", payload.TraceId),
            new Claim("env", payload.Environment),
            new Claim("role_name", payload.RoleName),
            new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: _options.ProductName,
            audience: GeneralConstants.Sr360Client,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
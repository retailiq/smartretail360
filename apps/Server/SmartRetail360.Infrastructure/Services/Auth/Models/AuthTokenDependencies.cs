using Microsoft.AspNetCore.Http;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Infrastructure.Common.DependencyInjection;

namespace SmartRetail360.Infrastructure.Services.Auth.Models;

public class AuthTokenDependencies : BaseDependencies
{
    public IRefreshTokenService RefreshTokenService { get; set; }
    public HttpContext HttpContext { get; set; }
    public IAccessTokenGenerator AccessTokenGenerator { get; set; }
}
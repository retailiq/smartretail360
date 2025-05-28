using Microsoft.AspNetCore.Http;
using SmartRetail360.Application.Interfaces.Auth;

namespace SmartRetail360.Infrastructure.Services.Auth.Models;

public class ConfirmTenantLoginDependencies : LoginDependencies
{
    public IRefreshTokenService RefreshTokenService { get; set; }
    public HttpContext HttpContext { get; set; }
}
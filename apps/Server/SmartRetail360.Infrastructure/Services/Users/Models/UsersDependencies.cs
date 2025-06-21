using Microsoft.AspNetCore.Http;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Infrastructure.Common.DependencyInjection;

namespace SmartRetail360.Infrastructure.Services.Users.Models;

public class UsersDependencies : BaseDependencies
{
    public UpdateUserProfileTokenGenerator UpdateUserProfileTokenGenerator { get; set; }
    public HttpContext HttpContext { get; set; }
}
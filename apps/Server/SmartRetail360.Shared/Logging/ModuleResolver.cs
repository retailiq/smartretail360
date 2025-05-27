using SmartRetail360.Shared.Constants;

namespace SmartRetail360.Shared.Logging;

public static class ModuleResolver
{
    public static string ResolveModule(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return LogSourceModules.CommonApi;

        path = path.ToLowerInvariant();

        return path switch
        {
            var p when p.Contains("/api/v1/users/register") => LogSourceModules.UserRegistrationService,
            var p when p.Contains("/api/v1/users/invite") => LogSourceModules.UserInvitationService,
            var p when p.Contains("/api/v1/notifications") => LogSourceModules.NotificationService,
            var p when p.Contains("/api/v1/auth/login") => LogSourceModules.UserLoginService,
            var p when p.Contains("/api/v1/auth/login/tenant") => LogSourceModules.ConfirmTenantLoginService,
            var p when p.Contains("/api/v1/auth/login/refresh") => LogSourceModules.RefreshTokenService,
            var p when p.Contains("/api/v1/auth/oauth/login") => LogSourceModules.OAuthLoginService,
            var p when p.Contains("/api/v1/auth") => LogSourceModules.AuthService,
            _ => LogSourceModules.CommonApi
        };
    }
}
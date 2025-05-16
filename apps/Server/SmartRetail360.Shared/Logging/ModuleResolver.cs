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
            var p when p.Contains("/api/v1/tenants") => LogSourceModules.RegisterTenantService,
            var p when p.Contains("/api/v1/auth") => LogSourceModules.AuthService,
            var p when p.Contains("/api/v1/products") => LogSourceModules.ProductService,
            var p when p.Contains("/api/v1/orders") => LogSourceModules.OrderService,
            var p when p.Contains("/api/v1/users") => LogSourceModules.UserService,
            _ => LogSourceModules.CommonApi
        };
    }
}
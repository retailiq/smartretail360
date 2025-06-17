using System.Text.RegularExpressions;
using SmartRetail360.Shared.Enums.AccessControl;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Shared.Contexts.Resolvers;

public static class EnvResolver
{
    public static string ResolveEnv(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return DefaultEnvironmentType.Default.GetEnumMemberValue();

        path = path.ToLowerInvariant();

        return path switch
        {
            var p when p.Contains("/server/api/dev/user") => DefaultEnvironmentType.Client.GetEnumMemberValue(),
            var p when p.Contains("/server/api/dev/headers") => DefaultEnvironmentType.Client.GetEnumMemberValue(),
            var p when p.Contains("/server/api/v1/users/register") => DefaultEnvironmentType.Client.GetEnumMemberValue(),
            var p when p.Contains("/server/api/v1/users/invite") => DefaultEnvironmentType.Client.GetEnumMemberValue(),
            var p when p.Contains("/server/api/v1/notifications") => DefaultEnvironmentType.Client.GetEnumMemberValue(),
            var p when p.Contains("/server/api/v1/auth/login/tenant") => DefaultEnvironmentType.Client.GetEnumMemberValue(),
            var p when p.Contains("/server/api/v1/auth/login") => DefaultEnvironmentType.Client.GetEnumMemberValue(),
            var p when p.Contains("/server/api/v1/auth/refresh") => DefaultEnvironmentType.Client.GetEnumMemberValue(),
            var p when p.Contains("/server/api/v1/auth/logout") => DefaultEnvironmentType.Client.GetEnumMemberValue(),
            var p when p.Contains("/server/api/v1/auth/validate-token") => DefaultEnvironmentType.Client.GetEnumMemberValue(),
            var p when p.Contains("/server/api/v1/auth/oauth/login") => DefaultEnvironmentType.Client.GetEnumMemberValue(),
            _ => DefaultEnvironmentType.Default.GetEnumMemberValue()
        };
    }
}
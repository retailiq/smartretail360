using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace SmartRetail360.Shared.Constants;

public static class TokenValidationRules
{
    // This dictionary contains regex patterns for whitelisting paths based on the service type.
    private static readonly ImmutableDictionary<string, string[]> RegexWhitelistByService =
        new Dictionary<string, string[]>
        {
            ["server"] =
            [
                // Specific paths
                @"^/server/api/v1/users/register$",
                // Subpaths under /server/api/v1/auth
                @"^/server/api/v1/auth(/.*)?$",
                @"^/server/api/v1/notifications(/.*)?$",
                // @"^/server/api/dev(/.*)?$",
                @"^/server/swagger(/.*)?$"
            ],
            ["data"] =
            [
                @"^/data/api/v1/public(/.*)?$",
                @"^/data/swagger(/.*)?$"
            ]
        }.ToImmutableDictionary();

    public static bool IsPathWhitelisted(string path)
    {
        foreach (var (_, patterns) in RegexWhitelistByService)
        {
            if (patterns.Any(pattern => Regex.IsMatch(path, pattern, RegexOptions.IgnoreCase)))
                return true;
        }

        return false;
    }
}

// Exact path matches only the specific route itself (no trailing segments)
// Wildcard path (with /.*)? supports any sub-paths and query parameters
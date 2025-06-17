using System.Text.RegularExpressions;
using SmartRetail360.ABAC.Configs;
using SmartRetail360.Shared.Constants;

namespace SmartRetail360.ABAC.Services;

public class AbacRouteMapper
{
    private readonly List<AbacRouteRule> _rules;

    public AbacRouteMapper(List<AbacRouteRule> rules)
    {
        _rules = rules;
    }

    public (string resourceType, string action) Map(string path)
    {
        foreach (var rule in _rules)
        {
            if (!string.IsNullOrWhiteSpace(rule.Pattern) &&
                Regex.IsMatch(path, rule.Pattern, RegexOptions.IgnoreCase))
            {
                return (rule.Resource, rule.Action);
            }
        }

        return (GeneralConstants.Unknown, GeneralConstants.Unknown);
    }
}
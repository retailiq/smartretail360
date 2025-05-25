namespace SmartRetail360.Shared.Utils;

public static class StringCaseConverter
{
    public static string ToSnakeCase(string input)
    {
        return string.Concat(
            input.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())
        ).ToLower();
    }

    public static string ToPascalCase(string input)
    {
        return string.Join("", input
            .Split('_')
            .Select(s => char.ToUpper(s[0]) + s.Substring(1)));
    }
}
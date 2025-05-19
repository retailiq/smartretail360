namespace SmartRetail360.Shared.Utils;

public static class SlugGenerator
{
    public static string GenerateSlug(string email)
    {
        return email.Split('@')[0].ToLower().Replace(".", "-").Replace("_", "-");
    }
}
using System.Globalization;

namespace SmartRetail360.Shared.Localization;

public static class CultureScope
{
    public static async Task RunWithCultureAsync(string? locale, Func<Task> action)
    {
        var culture = new CultureInfo(locale ?? "en");
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUICulture = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            await action();
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUICulture;
        }
    }
}
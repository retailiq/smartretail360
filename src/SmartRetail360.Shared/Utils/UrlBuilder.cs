namespace SmartRetail360.Shared.Utils;

public static class UrlBuilder
{
    public static string BuildApiUrl(string baseUrl, int version, string path, Dictionary<string, string>? queryParams = null)
    {
        if (!baseUrl.EndsWith('/'))
            baseUrl += "/";

        var versionedPath = $"server/api/v{version}/{path.TrimStart('/')}";
        var uri = new Uri(new Uri(baseUrl), versionedPath);

        if (queryParams == null || queryParams.Count == 0)
            return uri.ToString();

        var query = string.Join("&", queryParams.Select(kv =>
            $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
        return $"{uri}?{query}";
    }
}
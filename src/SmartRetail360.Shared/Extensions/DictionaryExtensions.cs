namespace SmartRetail360.Shared.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dict,
        TKey key,
        TValue fallback)
    {
        return dict.TryGetValue(key, out var value) ? value : fallback;
    }
}
using System.Text.Json;

namespace SmartRetail360.Shared.Utils;

public static class JsonSerializerHelper
{
    public static readonly JsonSerializerOptions IndentedOptions = new JsonSerializerOptions
    {
        WriteIndented = true
    };
}
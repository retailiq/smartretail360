using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Shared.Utils;

public static class TraceIdPrefix
{
    private static readonly Dictionary<TraceModule, string> PrefixMap = new()
    {
        { TraceModule.General, "general" },
        { TraceModule.Auth, "auth" },
        { TraceModule.Tenant, "tenant" },
        { TraceModule.Ai, "ai" },
        { TraceModule.Job, "job" },
        { TraceModule.Product, "product" },
    };

    public static string Get(TraceModule module) => PrefixMap[module];
}
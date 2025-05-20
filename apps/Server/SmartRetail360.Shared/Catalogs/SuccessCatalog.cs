using SmartRetail360.Shared.Constants;

namespace SmartRetail360.Shared.Catalogs;

public static class SuccessCatalog
{
    private static readonly Dictionary<int, string> _keys = new()
    {
        // Auth
        { SuccessCodes.PasswordChanged, "PasswordChanged" },
        
        // Account
        { SuccessCodes.TenantRegistered, "TenantRegistered" },
        { SuccessCodes.AccountActivatedSuccessfully, "AccountActivatedSuccessfully"},

        // Email-related
        { SuccessCodes.EmailResent, "EmailResent" },
    };

    public static string GetKey(int code)
    {
        return _keys.TryGetValue(code, out var key) ? key : "UnknownSuccess";
    }
}
using SmartRetail360.Shared.Constants;

namespace SmartRetail360.Shared.Catalogs;

public static class SuccessCatalog
{
    private static readonly Dictionary<int, string> _keys = new()
    {
        // File operations
        { SuccessCodes.UploadSuccess, "UploadSuccess" },
        { SuccessCodes.FileParsed, "FileParsed" },

        // User & tenant management
        { SuccessCodes.UserCreated, "UserCreated" },
        { SuccessCodes.TenantRegistered, "TenantRegistered" },
        { SuccessCodes.PasswordChanged, "PasswordChanged" },
        { SuccessCodes.EmailResent, "EmailResent" },
        { SuccessCodes.AccountActivatedSuccessfully, "AccountActivatedSuccessfully"},

        // Reporting & AI processing
        { SuccessCodes.ReportGenerated, "ReportGenerated" },
        { SuccessCodes.PredictionCompleted, "PredictionCompleted" }
    };

    public static string GetKey(int code)
    {
        return _keys.TryGetValue(code, out var key) ? key : "UnknownSuccess";
    }
}
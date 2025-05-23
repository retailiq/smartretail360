namespace SmartRetail360.Shared.Utils;

public static class TokenGenerator
{
    public static string GenerateActivateAccountToken()
    {
        return Guid.NewGuid().ToString("N");
    }
}
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Shared.Redis;

public static class RedisKeys
{
    public static string ResendAccountActivationEmail(string email)
        => $"email_limit:activate:{email}";
    
    public static string VerifyEmailRateLimit(string token)
        => $"email_limit:verify:{token}";

    public static string LogSampling(LogEventType eventType, string reason)
        =>$"log:sampling:{eventType}:{reason}";
    
    public static string RegisterAccountLock(string email)
        => $"lock:register:{email}";
    
    public static string UserLoginLock(string email)
        => $"lock:login:{email}";
    
    public static string SystemRole(string roleName)
        => $"role:{roleName}";
    
    public static string ActivationToken(string token)
        => $"token_activation:{token}";
}
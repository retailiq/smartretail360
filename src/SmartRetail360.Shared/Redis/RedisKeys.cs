using SmartRetail360.Shared.Constants;
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

    public static string PasswordReset(string email)
        => $"email_limit:reset:{email}";
    
    public static string LoginLock(string userId)
        => $"login_attempt:{userId}";

    public static string RegisterAccountLock(string email)
        => $"lock:register:{email}";
}
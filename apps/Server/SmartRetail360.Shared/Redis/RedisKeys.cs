namespace SmartRetail360.Shared.Redis;

public static class RedisKeys
{
    public static string ResendAccountActivationEmail(string email)
        => $"email_limit:activate:{email}";

    public static string PasswordReset(string email)
        => $"email_limit:reset:{email}";

    public static string SmsCode(string phone)
        => $"sms_code:{phone}";

    public static string LoginLock(string userId)
        => $"login_attempt:{userId}";

    public static string RegisterAccountLock(string email)
        => $"lock:register:{email}";

    public static string TaskStatus(Guid taskId)
        => $"task_status:{taskId}";
}
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Shared.Redis;

public static class RedisKeys
{
    public static string ResendAccountActivationEmail(string email)
        => $"email_limit:activate:{email}";

    public static string VerifyEmailRateLimit(string token)
        => $"email_limit:verify:{token}";

    public static string LogSampling(LogEventType eventType, string reason)
        => $"log:sampling:{eventType}:{reason}";

    public static string RegisterAccountLock(string email)
        => $"lock:register:{email}";

    public static string UserLoginLock(string email)
        => $"lock:login:{email}";

    public static string UserLoginSecurityLock(string email)
        => $"lock:login:security:{email}";

    public static string UserLoginFailures(string email)
        => $"lock:login:failures{email}";

    public static string SystemRole(string roleName)
        => $"role:{roleName}";

    public static string ActivationToken(string token)
        => $"token_activation:{token}";

    public static string AbacResourceType(string name)
        => $"abac:resource_type:{name}";

    public static string AbacAction(string name)
        => $"abac:action:{name}";

    public static string AbacEnvironment(string name)
        => $"abac:environment:{name}";

    public static string AbacPolicy(Guid tenantId, string resourceType, string action, string environment) =>
        $"abac:policy:{tenantId}:{resourceType}:{action}:{environment}";
    
    public static string AbacResourceGroup(string name) => $"abac:resource-group:{name}";
    public static string AbacPolicyTemplate(string name) => $"abac:policy-template:{name}";
}
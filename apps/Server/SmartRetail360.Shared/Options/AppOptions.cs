namespace SmartRetail360.Shared.Options;

public class AppOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string FrontendUrl { get; set; } = string.Empty;
    public string SupportEmail { get; set; } = "support@example.com";
    public bool EnableRegistration { get; set; } = true;
    public string DefaultCulture { get; set; } = "en";
    public List<string> SupportedCultures { get; set; } = new();
    public string EmailVerificationUrl { get; set; } = string.Empty;
    public double EmailSendLimitMinutes { get; set; } = 3;
    public double RegistrationLockTtlSeconds { get; set; } = 10;
    public double LogSamplingLimitMinutes { get; set; } = 2;
    public long RequestTimeoutThresholdMs { get; set; } = 3000;
    public int RequestBodyMaxLength { get; set; } = 2048;
    public bool EnableRequestBodyLogging { get; set; } = true;
    public bool EnableSensitiveFieldMasking { get; set; } = true;
    public List<string> SensitiveFields { get; set; } = new() { "password", "token", "apiKey" };
    public double AccountActivationLimitMinutes { get; set; } = 1;
    public int ActivationTokenLimitMinutes { get; set; } = 15;
    public double UserLoginLockTtlSeconds { get; set; } = 10;
    public string JwtSecret { get; set; } = string.Empty;
    public int JwtExpirySeconds { get; set; } = 3600;
}

// Always use { get; set; } in options classes — binding requires a public setter, even for read-only values.
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
    public int EmailValidityPeriodMinutes { get; set; } = 15;
    public double UserLoginLockTtlSeconds { get; set; } = 10;
    public string JwtSecret { get; set; } = string.Empty;
    public int UserLoginFailureThreshold { get; set; } = 3;
    public int RefreshTokenExpiryDaysWhenStaySignedIn { get; set; } = 30;
    public int RefreshTokenExpiryDaysDefault { get; set; } = 7;
    public int AccessTokenExpirySeconds { get; set; } = 1200; // 20 minutes
    public string RefreshTokenPath { get; set; } = "/api/v1/auth/refresh";
    public string CookieDomain { get; set; } = "example.com";
    public string LogoUrl { get; set; } = "https://example.com/logo.png";

    public string ProductName { get; set; } = "SmartRetail360";

    public OAuthAppConfig OAuth { get; set; } = new();
}

public class OAuthAppConfig
{
    public string Tenant { get; set; } = string.Empty;
    public List<string> RedirectUriWhitelist { get; set; } = new();
}

// Always use { get; set; } in options classes — binding requires a public setter, even for read-only values.
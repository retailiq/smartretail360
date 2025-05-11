namespace SmartRetail360.Shared.Options;

public class AppOptions
{
    /// <summary>The public root URL of the backend API (used for email links and callbacks)</summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>The frontend application URL (e.g. for activation success redirection)</summary>
    public string FrontendUrl { get; set; } = string.Empty;

    /// <summary>Support or notification email address</summary>
    public string SupportEmail { get; set; } = "support@example.com";

    /// <summary>Controls whether tenant self-registration is allowed</summary>
    public bool EnableRegistration { get; set; } = true;

    /// <summary>Default locale of the system (e.g. zh-CN, en-US)</summary>
    public string DefaultCulture { get; set; } = "en";
    public List<string> SupportedCultures { get; set; } = new();
    
    public string EmailVerificationUrl { get; set; } = string.Empty;
    
    public string EmailSendLimitMinutes { get; set; } = "3";
}
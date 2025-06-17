namespace SmartRetail360.Contracts.Auth.Requests;

public class UpdateAbacPolicyStatusRequest
{
    public bool? IsEnabled { get; set; }
}

// Must be nullable, because ASP.NET Core will set it tp false during model binding if not provided.
// Use fluent validation to ensure it is not null.
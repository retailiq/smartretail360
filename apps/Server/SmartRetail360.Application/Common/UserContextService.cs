using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Common;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _http;
    private readonly ILogger<UserContextService> _logger;

    public UserContextService(IHttpContextAccessor httpContextAccessor, ILogger<UserContextService> logger)
    {
        _http = httpContextAccessor;
        _logger = logger;

        // Initialize properties from HttpContext
        if (_http.HttpContext != null)
        {
            UserId = TryParseGuid("UserId");
            TenantId = TryParseGuid("TenantId");
            RoleId = TryParseGuid("RoleId");
            TraceId = Get("TraceId");
            Locale = Get("Locale");
            ClientEmail = Get("ClientEmail");

            var accType = Get("AccountType");
            if (Enum.TryParse<AccountType>(accType, out var parsed))
                AccountType = parsed;

            IpAddress = ResolveIpAddress();
        }
    }

    private string? Get(string key)
    {
        return _http.HttpContext?.Items[key]?.ToString();
    }

    private Guid? TryParseGuid(string key)
    {
        var value = Get(key);
        return Guid.TryParse(value, out var parsed) ? parsed : null;
    }

    private string ResolveIpAddress()
    {
        var context = _http.HttpContext;
        if (context == null)
            return "unknown";

        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            return forwarded.ToString().Split(',')[0].Trim();

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    // Change the properties to public setters to allow setting them from the constructor
    public Guid? UserId { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? RoleId { get; set; }
    public string? TraceId { get; set; }
    public string? Locale { get; set; }
    public string? Module { get; set; }
    public string? ClientEmail { get; set; }
    public AccountType? AccountType { get; set; }
    public string IpAddress { get; } = GeneralConstants.Unknown;
    public string? ErrorStack { get; set; }
    
    public void Inject(
        Guid? userId = null,
        Guid? tenantId = null,
        Guid? roleId = null,
        string? traceId = null,
        string? locale = null,
        string? module = null,
        string? clientEmail = null,
        AccountType? accountType = null,
        string? errorStack = null)
    {
        if (userId != null) UserId = userId;
        if (tenantId != null) TenantId = tenantId;
        if (roleId != null) RoleId = roleId;
        if (!string.IsNullOrWhiteSpace(traceId)) TraceId = traceId;
        if (!string.IsNullOrWhiteSpace(locale)) Locale = locale;
        if (!string.IsNullOrWhiteSpace(module)) Module = module;
        if (!string.IsNullOrWhiteSpace(clientEmail)) ClientEmail = clientEmail;
        if (accountType != null) AccountType = accountType;
        if (!string.IsNullOrWhiteSpace(errorStack)) ErrorStack = errorStack;
    }
}
using Microsoft.AspNetCore.Http;
using SmartRetail360.Shared.Constants;

namespace SmartRetail360.Application.Common.UserContext;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _http;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _http = httpContextAccessor;

        // Initialize properties from HttpContext
        if (_http.HttpContext != null)
        {
            UserId = TryParseGuid("UserId");
            TenantId = TryParseGuid("TenantId");
            RoleId = TryParseGuid("RoleId");
            TraceId = Get("TraceId");
            Locale = Get("Locale");
            Email = Get("Email");
            UserName = Get("UserName");
            IpAddress = ResolveIpAddress();
        }
        
        LogId = Guid.NewGuid().ToString();
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
            return GeneralConstants.Unknown;

        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            return forwarded.ToString().Split(',')[0].Trim();

        return context.Connection.RemoteIpAddress?.ToString() ?? GeneralConstants.Unknown;
    }

    // Change the properties to public setters to allow setting them from the constructor
    public Guid? UserId { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? RoleId { get; set; }
    public string? TraceId { get; set; }
    public string? Locale { get; set; }
    public string? Module { get; set; }
    public string? Email { get; set; }
    public string IpAddress { get; set; } = GeneralConstants.Unknown;
    public string? ErrorStack { get; set; }
    public string? Action { get; set; }
    public string? RoleName { get; set; }
    public string? LogId { get; set; }
    public string? UserName { get; set; }

    public void Inject(
        Guid? userId = null,
        Guid? tenantId = null,
        Guid? roleId = null,
        string? traceId = null,
        string? locale = null,
        string? module = null,
        string? email = null,
        string? errorStack = null,
        string? ipAddress = null,
        string? action = null,
        string? roleName = null,
        string? logId = null,
        string? userName = null)
    {
        if (userId != null) UserId = userId;
        if (tenantId != null) TenantId = tenantId;
        if (roleId != null) RoleId = roleId;
        if (!string.IsNullOrWhiteSpace(traceId)) TraceId = traceId;
        if (!string.IsNullOrWhiteSpace(locale)) Locale = locale;
        if (!string.IsNullOrWhiteSpace(module)) Module = module;
        if (!string.IsNullOrWhiteSpace(email)) Email = email;
        if (!string.IsNullOrWhiteSpace(errorStack)) ErrorStack = errorStack;
        if (!string.IsNullOrWhiteSpace(ipAddress)) IpAddress = ipAddress;
        if (!string.IsNullOrWhiteSpace(action)) Action = action;
        if (!string.IsNullOrWhiteSpace(roleName)) RoleName = roleName;
        if (!string.IsNullOrWhiteSpace(logId)) LogId = logId;
        if (!string.IsNullOrWhiteSpace(userName)) UserName = userName;
    }
}
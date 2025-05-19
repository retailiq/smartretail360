// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.Logging;
// using SmartRetail360.Application.Interfaces.Common;
// using SmartRetail360.Shared.Enums;
//
// namespace SmartRetail360.Application.Common;
//
// public class UserContextService : IUserContextService
// {
//     private readonly IHttpContextAccessor _http;
//     private readonly ILogger<UserContextService> _logger;
//
//     public UserContextService(IHttpContextAccessor httpContextAccessor, ILogger<UserContextService> logger)
//     {
//         _http = httpContextAccessor;
//         _logger = logger;
//     }
//     
//     private string? Get(string key)
//     {
//         return _http.HttpContext?.Items[key]?.ToString();
//     }
//     
//     public Guid? UserId => TryParseGuid("UserId");
//     public Guid? TenantId => TryParseGuid("TenantId");
//     public Guid? RoleId => TryParseGuid("RoleId");
//     public string? TraceId => Get("TraceId");
//     public string? Locale => Get("Locale");
//     public string? Module { get; set; }
//     public string? ClientEmail => Get("ClientEmail");
//     public AccountType? AccountType
//     {
//         get
//         {
//             var value = Get("AccountType");
//             return Enum.TryParse<AccountType>(value, out var parsed) ? parsed : null;
//         }
//     }
//     public string IpAddress
//     {
//         get
//         {
//             var context = _http.HttpContext;
//
//             if (context == null)
//                 return "unknown";
//
//             // 优先检查是否有代理转发头
//             if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
//             {
//                 return forwarded.ToString().Split(',')[0].Trim(); // 拿第一个 IP
//             }
//
//             // 否则取直连 IP
//             return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
//         }
//     }
//     
//     private Guid? TryParseGuid(string key)
//     {
//         var value = Get(key);
//         return Guid.TryParse(value, out var parsed) ? parsed : null;
//     }
//     
//     public void LogAllContext()
//     {
//         var context = _http.HttpContext;
//         if (context?.Items != null)
//         {
//             foreach (var kvp in context.Items)
//             {
//                 _logger.LogInformation("[UserContext] {Key} = {Value}", kvp.Key, kvp.Value);
//             }
//         }
//     }
// }

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SmartRetail360.Application.Interfaces.Common;
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

        // 初始化默认值（如果有 HttpContext 可用）
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

    // 改成可读写属性
    public Guid? UserId { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? RoleId { get; set; }
    public string? TraceId { get; set; }
    public string? Locale { get; set; }
    public string? Module { get; set; }
    public string? ClientEmail { get; set; }
    public AccountType? AccountType { get; set; }
    public string IpAddress { get; set; } = "unknown";

    public void LogAllContext()
    {
        var context = _http.HttpContext;
        if (context?.Items != null)
        {
            foreach (var kvp in context.Items)
            {
                _logger.LogInformation("[UserContext] {Key} = {Value}", kvp.Key, kvp.Value);
            }
        }
    }
}
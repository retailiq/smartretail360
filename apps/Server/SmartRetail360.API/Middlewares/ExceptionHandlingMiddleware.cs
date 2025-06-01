using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SmartRetail360.Application.Common.UserContext;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Exceptions;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IOptions<JsonOptions> jsonOptionsAccessor
    )
    {
        _next = next;
        _logger = logger;
        _jsonOptions = jsonOptionsAccessor.Value.JsonSerializerOptions;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // call the next middleware
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var userContext = context.RequestServices.GetService<IUserContextService>();
        var localizer = context.RequestServices.GetRequiredService<MessageLocalizer>();
        
        (int statusCode, int errorCode) = ex switch
        {
            CommonException commonEx =>
                (commonEx.StatusCode, commonEx.ErrorCode),

            SecurityException secEx =>
                (secEx.StatusCode, secEx.ErrorCode),
            
            MailKit.Net.Smtp.SmtpCommandException smtpEx => 
                ((int)HttpStatusCode.ServiceUnavailable, ErrorCodes.EmailSendFailed),

            MailKit.Net.Smtp.SmtpProtocolException protoEx => 
                ((int)HttpStatusCode.ServiceUnavailable, ErrorCodes.EmailSendFailed),
            
            Npgsql.NpgsqlException or Npgsql.PostgresException =>
                ((int)HttpStatusCode.ServiceUnavailable, ErrorCodes.DatabaseUnavailable),

            DbUpdateException dbEx when dbEx.InnerException is Npgsql.NpgsqlException =>
                ((int)HttpStatusCode.ServiceUnavailable, ErrorCodes.DatabaseUnavailable),

            _ => ((int)HttpStatusCode.InternalServerError, ErrorCodes.InternalServerError)
        };

        context.Response.StatusCode = statusCode;
        
        var traceId = userContext?.TraceId;
        var message = localizer.GetErrorMessage(errorCode);
        
        var response = ApiResponse<object>.Fail(errorCode, message, traceId);
        var json = JsonSerializer.Serialize(response, _jsonOptions);
        await context.Response.WriteAsync(json);
    }
}


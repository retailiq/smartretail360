using System.Text.Json.Serialization;

namespace SmartRetail360.Shared.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? TraceId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ErrorInfo? Error { get; set; }

    public static ApiResponse<T> Ok(T? data, string? message = null, string? traceId = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            TraceId = traceId,
            Data = data
        };
    }

    public static ApiResponse<T> Fail(int code, string details, string? traceId = null, T? data = default)
    {
        return new ApiResponse<T>
        {
            Success = false,
            TraceId = traceId,
            Data = data,
            Error = new ErrorInfo { Code = code, Details = details }
        };
    }
}
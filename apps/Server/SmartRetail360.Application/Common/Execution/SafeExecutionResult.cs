using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Application.Common.Execution;

public class SafeExecutionResult
{
    public bool IsSuccess { get; set; }
    private ApiResponse<object> Response { get; set; } = null!;

    public static SafeExecutionResult Success(string? message = null, string? traceId = null)
    {
        return new SafeExecutionResult
        {
            IsSuccess = true,
            Response = ApiResponse<object>.Ok(null, message, traceId)
        };
    }

    public static SafeExecutionResult Fail(int errorCode, string errorMessage, string? traceId = null)
    {
        return new SafeExecutionResult
        {
            IsSuccess = false,
            Response = ApiResponse<object>.Fail(errorCode, errorMessage, traceId)
        };
    }

    public ApiResponse<object> ToObjectResponse()
    {
        return Response;
    }
}

public class SafeExecutionResult<T>
{
    public bool IsSuccess { get; set; }
    public ApiResponse<T> Response { get; set; } = null!;

    public static SafeExecutionResult<T> Success(T data, string? message = null, string? traceId = null)
    {
        return new SafeExecutionResult<T>
        {
            IsSuccess = true,
            Response = ApiResponse<T>.Ok(data, message, traceId)
        };
    }

    public static SafeExecutionResult<T> Fail(int errorCode, string errorMessage, string? traceId = null)
    {
        return new SafeExecutionResult<T>
        {
            IsSuccess = false,
            Response = ApiResponse<T>.Fail(errorCode, errorMessage, traceId)
        };
    }

    public ApiResponse<object> ToObjectResponse()
    {
        if (IsSuccess)
        {
            return ApiResponse<object>.Ok(null, Response.Message, Response.TraceId);
        }

        var errorCode = Response.Error?.Code ?? ErrorCodes.UnknownError;
        var errorDetails = Response.Error?.Details ?? GeneralConstants.Unknown;

        return ApiResponse<object>.Fail(errorCode, errorDetails, Response.TraceId);
    }
}
using SmartRetail360.Shared.Constants;

namespace SmartRetail360.Shared.Responses;

public static class ApiResponseExtensions
{
    public static ApiResponse<T> To<T>(this ApiResponse<object> response, T? data = default)
    {
        return response.Success
            ? ApiResponse<T>.Ok(data, response.Message, response.TraceId)
            : ApiResponse<T>.Fail(response.Error!.Code, response.Error.Details ?? GeneralConstants.Unknown, response.TraceId, data);
    }
}
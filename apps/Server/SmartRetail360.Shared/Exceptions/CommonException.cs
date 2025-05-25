using System.Net;

namespace SmartRetail360.Shared.Exceptions;

public class CommonException : Exception
{
    public int ErrorCode { get; }
    public int StatusCode { get; }

    public CommonException(int errorCode = 10000, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : base()
    {
        ErrorCode = errorCode;
        StatusCode = (int)statusCode;
    }
}
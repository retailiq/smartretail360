using System.Net;

namespace SmartRetail360.Shared.Exceptions;

public class SecurityException : Exception
{
    public int ErrorCode { get; }
    public int StatusCode { get; }

    public SecurityException(int errorCode, HttpStatusCode statusCode = HttpStatusCode.Forbidden)
    {
        ErrorCode = errorCode;
        StatusCode = (int)statusCode;
    }
}
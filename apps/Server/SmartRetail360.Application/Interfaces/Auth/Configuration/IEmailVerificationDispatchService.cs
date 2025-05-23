using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Application.Interfaces.Auth.Configuration;

public interface IEmailVerificationDispatchService
{
    Task<ApiResponse<object>> DispatchAsync(string token);
}
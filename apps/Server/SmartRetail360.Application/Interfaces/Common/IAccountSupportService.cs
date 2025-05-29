using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Application.Interfaces.Common;

public interface IAccountSupportService
{
    Task<(List<AccountActivationToken>?, ApiResponse<object>?)> GetActivationTokenListAsync(Guid userId);
}
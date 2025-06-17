using Microsoft.EntityFrameworkCore;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Execution;
using SmartRetail360.Persistence;
using SmartRetail360.Persistence.Data;
using SmartRetail360.Platform.Interfaces;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Platform.Services;

public class AccountSupportService : IAccountSupportService
{
    private readonly AppDbContext _db;
    private readonly ISafeExecutor _safeExecutor;

    public AccountSupportService(AppDbContext db, ISafeExecutor safeExecutor)
    {
        _db = db;
        _safeExecutor = safeExecutor;
    }

    public async Task<(List<AccountActivationToken>?, ApiResponse<object>?)> GetActivationTokenListAsync(Guid userId)
    {
        var result = await _safeExecutor.ExecuteAsync(
            () => _db.AccountActivationTokens
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );

        return (result.Response.Data, result.IsSuccess ? null : result.ToObjectResponse());
    }
}
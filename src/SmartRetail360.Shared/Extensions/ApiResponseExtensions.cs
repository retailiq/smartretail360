using Microsoft.AspNetCore.Mvc;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Shared.Extensions;

public static class ApiResponseExtensions
{
    public static IActionResult ToHttpResult<T>(this ApiResponse<T> response, int successStatusCode = 200)
    {
        if (response.Success)
            return new ObjectResult(response) { StatusCode = successStatusCode };

        var statusCode = MapErrorCodeToStatusCode(response.Error?.Code);
        return new ObjectResult(response) { StatusCode = statusCode };
    }

    private static int MapErrorCodeToStatusCode(int? code)
    {
        return code switch
        {
            // 400 BadRequest
            ErrorCodes.InvalidPassword or
                ErrorCodes.InvalidToken or
                ErrorCodes.TokenAlreadyUsed or
                ErrorCodes.TokenRevoked or
                ErrorCodes.PasswordEmailMismatch or
                ErrorCodes.InvalidOAuthProvider or
                ErrorCodes.OAuthUserProfileFetchFailed or
                ErrorCodes.OAuthUserProfileNotExists or
                ErrorCodes.RefreshTokenMissing or
                ErrorCodes.AuthorizationFailure or
                ErrorCodes.InvalidAbacPolicyRule or
                ErrorCodes.TokenMissing or
                ErrorCodes.TokenValidationFailed or
                ErrorCodes.AccountNotActivatedOrInvitationPending or
                ErrorCodes.EmailExists or
                ErrorCodes.TooFrequentEmailRequest => 400,

            // 401 Unauthorized
            ErrorCodes.TokenExpired or
                ErrorCodes.AccountNotActivated or
                ErrorCodes.AccountSuspended or
                ErrorCodes.AccountLocked or
                ErrorCodes.AccountBanned or
                ErrorCodes.AccountDeleted or
                ErrorCodes.RefreshTokenExpired or
                ErrorCodes.RefreshTokenReplayDetected or
                ErrorCodes.RefreshTokenRevoked => 401,

            // 403 Forbidden
            ErrorCodes.AuthorizationFailure or
                ErrorCodes.AllTenantsDisabled or
                ErrorCodes.TenantDisabled or
                ErrorCodes.TenantUserDisabled or
                ErrorCodes.AccountExistsButNotActivated => 403,

            // 404 Not Found
            ErrorCodes.AccountNotFound or
                ErrorCodes.TenantNotFound or
                ErrorCodes.TokenNotFound or
                ErrorCodes.TenantUserRecordNotFound or
                ErrorCodes.EmailTemplateNotFound or
                ErrorCodes.EmailStrategyNotFound or
                ErrorCodes.AbacPolicyNotFound => 404,

            // 409 Conflict
            ErrorCodes.DuplicateRegisterAttempt or
                ErrorCodes.DuplicateLoginAttempt or
                ErrorCodes.AbacPolicyDisabled or
                ErrorCodes.AbacPolicyHasBeenReplaced or
                ErrorCodes.AccountAlreadyActivated => 409,

            // 429 Too Many Requests
            ErrorCodes.TooFrequentActivationAttempt or
                ErrorCodes.TooFrequentEmailRequest => 429,

            // 500 Internal Server Error (默认兜底)
            ErrorCodes.InternalServerError or
                ErrorCodes.DatabaseUnavailable or
                ErrorCodes.EmailSendFailed or
                ErrorCodes.UnknownError or
                null => 500,

            _ => 400 // fallback
        };
    }
}
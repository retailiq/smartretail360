using SmartRetail360.Contracts.Users.Requests;
using SmartRetail360.Contracts.Users.Responses;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Application.Interfaces.Users;

public interface IUserProfileUpdateService
{
    Task<ApiResponse<UpdateUserBasicProfileResponse>> UpdateUserBasicProfile(UpdateUserBasicProfileRequest request, Guid userId);
    Task<ApiResponse<object>> UpdateUserPassword(UpdateUserPasswordRequest request, Guid userId);
    Task<ApiResponse<UpdateUserEmailResponse>> UpdateUserEmail(UpdateUserEmailRequest request, Guid userId);
}
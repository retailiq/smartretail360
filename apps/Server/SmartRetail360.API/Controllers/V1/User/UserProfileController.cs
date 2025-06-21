using Microsoft.AspNetCore.Mvc;
using SmartRetail360.Application.Interfaces.Users;
using SmartRetail360.Contracts.Users.Requests;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.API.Controllers.V1.AccountRegistration;

[ApiController]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v{version:apiVersion}/users")]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileUpdateService _updateService;

    public UserProfileController(
        IUserProfileUpdateService updateService
    )
    {
        _updateService = updateService;
    }

    [HttpPatch("{userId:guid}/profile/edit")]
    public async Task<IActionResult> UpdateUserBasicProfile(Guid userId,
        [FromBody] UpdateUserBasicProfileRequest request)
    {
        var result = await _updateService.UpdateUserBasicProfile(request, userId);
        return result.ToHttpResult();
    }

    [HttpPut("{userId:guid}/password/edit")]
    public async Task<IActionResult> UpdateUserPassword(Guid userId,
        [FromBody] UpdateUserPasswordRequest request)
    {
        var result = await _updateService.UpdateUserPassword(request, userId);
        return result.ToHttpResult();
    }

    [HttpPut("{userId:guid}/email/edit")]
    public async Task<IActionResult> UpdateUserEmail(Guid userId, [FromBody] UpdateUserEmailRequest request)
    {
        var result = await _updateService.UpdateUserEmail(request, userId);
        return result.ToHttpResult();
    }
}
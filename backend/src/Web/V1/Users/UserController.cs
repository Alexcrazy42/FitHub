using FitHub.Application.Users;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Users;

public class UserController : ControllerBase
{
    private readonly IUserService userService;
    private readonly ICurrentIdentityUserIdAccessor accessor;

    public UserController(IUserService userService, ICurrentIdentityUserIdAccessor accessor)
    {
        this.userService = userService;
        this.accessor = accessor;
    }

    [HttpGet(ApiRoutesV1.Me)]
    [Authorize]
    public async Task<UserResponse> GetCurrentUser(CancellationToken ct)
    {
        var userId = accessor.GetCurrentUserId();
        var user = await userService.GetUserAsync(userId, ct);
        return user.ToResponse();
    }
}

using FitHub.Application.Users;
using FitHub.Application.Users.Commands;
using FitHub.Authentication;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Users;
using FitHub.Web.Common;
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

    [HttpGet(ApiRoutesV1.Users)]
    [Authorize]
    public async Task<ListResponse<UserResponse>> GetUsers([FromQuery] GetUsersRequest? request, [FromQuery] PagedRequest? pagedRequest, CancellationToken ct)
    {
        var query = new GetUserQuery() { PartName = request?.PartName, };
        var pagedQuery = pagedRequest.ToQuery();
        var userResult = await userService.GetUsersAsync(query, pagedQuery, ct);

        return userResult.ToListResponse(UserExtensions.ToResponse);
    }
}

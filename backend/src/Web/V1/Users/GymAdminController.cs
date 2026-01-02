using FitHub.Application.Users;
using FitHub.Application.Users.GymAdmins;
using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Users;
using FitHub.Contracts.V1.Users.GymAdmins;
using FitHub.Domain.Users;
using FitHub.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Users;

public class GymAdminController : ControllerBase
{
    private readonly IUserService userService;
    private readonly IGymAdminService gymAdminService;
    private readonly IAccessService accessService;
    private readonly ICurrentIdentityUserIdAccessor accessor;

    public GymAdminController(IUserService userService, IGymAdminService gymAdminService, IAccessService accessService, ICurrentIdentityUserIdAccessor accessor)
    {
        this.userService = userService;
        this.gymAdminService = gymAdminService;
        this.accessService = accessService;
        this.accessor = accessor;
    }

    [HttpGet(ApiRoutesV1.GymAdminMe)]
    [Authorize(Policy = AuthorizationPolicies.GymAdminOnly)]
    public async Task<GymAdminResponse> GetMe(CancellationToken ct)
    {
        var userId = accessor.GetCurrentUserId();
        var gymAdmin = await gymAdminService.GetByUserIdAsync(userId, ct);
        return gymAdmin.ToResponse();
    }

    [HttpGet(ApiRoutesV1.GymAdmins)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task<ListResponse<GymAdminResponse>> GetAll([FromQuery] PagedRequest? paged, CancellationToken ct)
    {
        var domain = paged.ToQuery();
        var gymAdminResult = await gymAdminService.GetAll(domain, ct);
        return gymAdminResult.ToListResponse(UserExtensions.ToResponse);
    }

    [HttpPost(ApiRoutesV1.GymAdmins)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task<UserResponse> CreateGymAdmin([FromBody] CreateGymAdminRequest? request, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(request, "request cannot be null");
        var user = await userService.RegisterGymAdminAsync(request, ct);
        return user.ToResponse();
    }

    [HttpPut(ApiRoutesV1.GymAdminSetStatus)]
    public async Task Deactivate([FromRoute] string? id, [FromQuery] bool? status, CancellationToken ct)
    {
        await accessService.EnsureHasAnyPolicyAsync(AuthorizationPolicies.CmsAdminOnly);
        var gymAdminId = GymAdminId.Parse(id);
        await gymAdminService.SetStatus(gymAdminId, status ?? false, ct);

    }
}

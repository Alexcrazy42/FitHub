using FitHub.Application.Users;
using FitHub.Application.Users.CmsAdmins;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Auth;
using FitHub.Contracts.V1.Users;
using FitHub.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Users;

public class CmsAdminController : ControllerBase
{
    private readonly IUserService userService;
    private readonly ICmsAdminService cmsAdminService;
    private readonly IAuthorizationService authorizationService;

    public CmsAdminController(IUserService userService, ICmsAdminService cmsAdminService, IAuthorizationService authorizationService)
    {
        this.userService = userService;
        this.cmsAdminService = cmsAdminService;
        this.authorizationService = authorizationService;
    }

    [HttpGet(ApiRoutesV1.CmsAdmins)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task<ListResponse<UserResponse>> Get([FromQuery] PagedRequest? paged, CancellationToken ct)
    {
        var query = paged.ToDomain();
        var usersResult = await cmsAdminService.GetCmsAdmins(query, ct);
        return usersResult.ToResponse(UserExtensions.ToResponse);
    }

    [HttpPost(ApiRoutesV1.CmsAdmins)]
    [Authorize(Policy = AuthorizationPolicies.CmsAdminOnly)]
    public async Task<UserResponse> CreateCmsAdmin([FromBody] CreateCmsAdminRequest? request, CancellationToken ct)
    {
        // var isCmsAdmin = (await authorizationService.AuthorizeAsync(User, AuthorizationPolicies.CmsAdminOnly)).Succeeded;
        // var isTrainer = (await authorizationService.AuthorizeAsync(User, AuthorizationPolicies.TrainerOnly)).Succeeded;

        ValidationException.ThrowIfNull(request, "request cannot be null");
        var user = await userService.RegisterCmsAdminAsync(request, ct);
        return user.ToResponse();
    }
}

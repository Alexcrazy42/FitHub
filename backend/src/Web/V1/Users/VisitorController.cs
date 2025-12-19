using FitHub.Application.Users.Visitors;
using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Users.Visitors;
using FitHub.Domain.Users;
using FitHub.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Users;

public class VisitorController : ControllerBase
{
    private readonly IVisitorService visitorService;
    private readonly IAccessService accessService;
    private readonly ICurrentIdentityUserIdAccessor accessor;

    public VisitorController(IVisitorService visitorService, IAccessService accessService, ICurrentIdentityUserIdAccessor accessor)
    {
        this.visitorService = visitorService;
        this.accessService = accessService;
        this.accessor = accessor;
    }

    [HttpGet(ApiRoutesV1.VisitorMe)]
    [Authorize(Policy = AuthorizationPolicies.GymVisitorOnly)]
    public async Task<VisitorResponse> GetMe(CancellationToken ct)
    {
        var userId = accessor.GetCurrentUserId();
        var visitor = await visitorService.GetByUserIdAsync(userId, ct);
        return visitor.ToResponse();
    }

    [HttpGet(ApiRoutesV1.Visitors)]
    public async Task<ListResponse<VisitorResponse>> Get([FromQuery] PagedRequest? pagedRequest, [FromQuery] VisitorSearchRequest? visitorRequest, CancellationToken ct)
    {
        await accessService.EnsureHasAnyPolicyAsync(AuthorizationPolicies.CmsAdminOnly, AuthorizationPolicies.GymAdminOnly);
        var domain = pagedRequest.ToDomain();
        var result = await visitorService.GetAll(domain, visitorRequest, ct);
        return result.ToResponse(UserExtensions.ToResponse);
    }

    [HttpPut(ApiRoutesV1.VisitorSetStatus)]
    public async Task SetStatus([FromRoute] string? id, [FromQuery] bool? status, CancellationToken ct)
    {
        await accessService.EnsureHasAnyPolicyAsync(AuthorizationPolicies.CmsAdminOnly, AuthorizationPolicies.GymAdminOnly);
        var visitorId = VisitorId.Parse(id);
        await visitorService.SetStatus(visitorId, status ?? false, ct);
    }
}

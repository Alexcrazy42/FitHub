using FitHub.Application.Users.Visitors;
using FitHub.Common.AspNetCore.Auth;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Users.Visitors;
using FitHub.Domain.Users;
using FitHub.Web.Common;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Users;

public class VisitorController : ControllerBase
{
    private readonly IVisitorService visitorService;
    private readonly IAccessService accessService;

    public VisitorController(IVisitorService visitorService, IAccessService accessService)
    {
        this.visitorService = visitorService;
        this.accessService = accessService;
    }

    [HttpGet(ApiRoutesV1.Visitors)]
    public async Task<ListResponse<VisitorResponse>> Get([FromQuery] PagedRequest? pagedRequest, CancellationToken ct)
    {
        await accessService.EnsureHasAnyPolicyAsync(AuthorizationPolicies.CmsAdminOnly, AuthorizationPolicies.GymAdminOnly);
        var domain = pagedRequest.ToDomain();
        var result = await visitorService.GetAll(domain, ct);
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

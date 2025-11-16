using FitHub.Application.Users.Visitors;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Users.Visitors;
using FitHub.Web.Common;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Users;

public class VisitorController : ControllerBase
{
    private readonly IVisitorService visitorService;

    public VisitorController(IVisitorService visitorService)
    {
        this.visitorService = visitorService;
    }

    [HttpGet(ApiRoutesV1.Visitors)]
    public async Task<ListResponse<VisitorResponse>> Get([FromQuery] PagedRequest? pagedRequest, CancellationToken ct)
    {
        var domain = pagedRequest.ToDomain();
        var result = await visitorService.GetAll(domain, ct);
        return result.ToResponse(UserExtensions.ToResponse);
    }
}

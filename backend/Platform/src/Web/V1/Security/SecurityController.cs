using FitHub.Application.Security;
using FitHub.Common.Utilities.System;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Security;

public class SecurityController : ControllerBase
{
    private readonly ILinkSecurityService linkSecurityService;

    public SecurityController(ILinkSecurityService linkSecurityService)
    {
        this.linkSecurityService = linkSecurityService;
    }

    [HttpPost(ApiRoutesV1.SecurityCheckUrl)]
    [Authorize]
    public async Task<CheckUrlResponse> CheckUrl([FromBody] CheckUrlRequest request, CancellationToken ct)
    {
        var result = await linkSecurityService.CheckUrlAsync(request.Url, ct);
        return new CheckUrlResponse
        {
            Domain = result.Domain,
            HasSsl = result.HasSsl,
            MaliciousCount = result.MaliciousCount,
            SuspiciousCount = result.SuspiciousCount,
            TotalEngines = result.TotalEngines,
            Status = result.Status.ToString()
        };
    }
}

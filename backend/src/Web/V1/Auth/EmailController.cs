using FitHub.Common.AspNetCore.Accounting;
using FitHub.Contracts.V1;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Auth;

public class EmailController : ControllerBase
{
    private readonly IIdentityUserService identityUserService;

    public EmailController(IIdentityUserService identityUserService)
    {
        this.identityUserService = identityUserService;
    }

    [HttpGet(ApiRoutesV1.EmailAvailable)]
    public async Task<bool> EmailAvailable([FromQuery] string email, CancellationToken ct)
    {
        var user = await identityUserService.GetByEmailAsync(email, ct);
        return user is null;
    }
}

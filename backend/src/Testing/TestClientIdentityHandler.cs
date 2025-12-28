using System.Net.Http.Headers;
using FitHub.Authentication;
using FitHub.Common.AspNetCore.Tokens;
using FitHub.Common.Identity.Client;

namespace FitHub.Common.Testing;

internal sealed class TestClientIdentityHandler : ClientIdentityHandlerBase
{
    private readonly ITokenService tokenService;
    private readonly ITestDesiredUserIdProvider desiredUserIdProvider;

    public TestClientIdentityHandler(ITokenService tokenService, ITestDesiredUserIdProvider desiredUserIdProvider)
    {
        this.tokenService = tokenService;
        this.desiredUserIdProvider = desiredUserIdProvider;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var claims = ITokenService.CreateCommonClaims(desiredUserIdProvider.GetUserId().Value.ToString(), "", IdentityUserType.CmsAdmin);
        var accessToken = tokenService.Create(claims);
        request.Headers.Authorization = new AuthenticationHeaderValue(AuthenticationScheme.Bearer, accessToken);
        return base.SendAsync(request, cancellationToken);
    }
}

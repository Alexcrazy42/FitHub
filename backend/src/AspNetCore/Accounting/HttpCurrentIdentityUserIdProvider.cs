using Microsoft.AspNetCore.Http;

namespace FitHub.Common.AspNetCore.Accounting;

internal sealed class HttpCurrentIdentityUserIdProvider : ICurrentIdentityUserIdProvider
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public HttpCurrentIdentityUserIdProvider(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public IdentityUserId? GetCurrentUserIdOrDefault()
        => IdentityUserId.TryParse(httpContextAccessor.HttpContext?.User.GetUserId(), out var id)
            ? id
            : null;

    public string? GetSessionId() => httpContextAccessor.HttpContext?.User.GetSessionId();
}

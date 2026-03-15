using FitHub.Common.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace FitHub.Common.AspNetCore.Auth;

public class AccessService : IAccessService
{
    private readonly IAuthorizationService authorizationService;
    private readonly IHttpContextAccessor httpContextAccessor;

    public AccessService(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
    {
        this.authorizationService = authorizationService;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task EnsureHasAnyPolicyAsync(params string[] policies)
    {
        var context = httpContextAccessor.HttpContext;

        if (context == null)
        {
            throw new UnexpectedException("HttpContext is not available.");
        }

        var user = context.User;

        if (user.Identity?.IsAuthenticated == false)
        {
            throw new ForbidException();
        }

        foreach (var policy in policies)
        {
            var result = await authorizationService.AuthorizeAsync(user, policy);
            if (result.Succeeded)
            {
                return;
            }
        }

        throw new ForbidException();
    }
}

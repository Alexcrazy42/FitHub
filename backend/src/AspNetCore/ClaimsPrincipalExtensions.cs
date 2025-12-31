using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FitHub.Common.AspNetCore;

public static class ClaimsPrincipalExtensions
{
    //private static readonly string[] UserIdClaimNames = { ClaimTypes.NameIdentifier, JwtRegisteredClaimNames.Sub };

    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        var userIdClaim = principal
            .Identities
            .SelectMany(identity => identity.Claims)
            .FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

        return userIdClaim?.Value;
    }

    public static string? GetUsername(this ClaimsPrincipal principal)
    {
        var userNameClaim = principal
            .Identities
            .SelectMany(identity => identity.Claims)
            .FirstOrDefault(claim => claim.Type == ClaimTypes.Name);

        return userNameClaim?.Value;
    }

    public static string? GetSessionId(this ClaimsPrincipal principal)
    {
        var sessionIdClaim = principal
            .Identities
            .SelectMany(identity => identity.Claims)
            .FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sid);

        return sessionIdClaim?.Value;
    }
}

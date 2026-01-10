using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FitHub.Authentication;
using FitHub.Common.Entities;

namespace FitHub.Common.AspNetCore;

public static class ClaimsPrincipalExtensions
{
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

    public static IdentityUserId GetParsedUserId(this ClaimsPrincipal principal)
    {
        var userId = principal.GetUserId();
        if (userId == null)
        {
            throw new UnexpectedException("UserId is null");
        }

        return IdentityUserId.Parse(userId);
    }
}

using System.Security.Claims;
using FitHub.Common.AspNetCore.Accounting;

namespace FitHub.Common.AspNetCore.Auth;

public static class AuthorizationExtensions
{
    public static bool HasUserType(this ClaimsPrincipal user, IdentityUserType requiredType)
    {
        var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;
        if (String.IsNullOrEmpty(roleClaim))
        {
            return false;
        }

        if (!Int32.TryParse(roleClaim, out var roleValue))
        {
            return false;
        }

        var userType = (IdentityUserType)roleValue;
        return userType.HasFlag(requiredType);
    }
}

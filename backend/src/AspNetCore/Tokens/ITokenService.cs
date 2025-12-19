using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;

namespace FitHub.Common.AspNetCore.Tokens;

public interface ITokenService
{
    ClaimsPrincipal Validate(string token);

    string Create(IReadOnlyList<Claim> claims);

    public static IReadOnlyList<Claim> CreateCommonClaims(string sub, string sessionId, IdentityUserType userType)
    {
        var rolesClaim = ((int)userType).ToString();
        return new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, sub),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sid, sessionId),
            new Claim(ClaimTypes.Role, rolesClaim)
        };
    }

}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FitHub.Common.AspNetCore.Tokens;

internal interface ITokenService
{
    ClaimsPrincipal Validate(string token);

    string Create(IReadOnlyList<Claim> claims);

    public static IReadOnlyList<Claim> CreateCommonClaims(string sub)
        => new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, sub),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

}

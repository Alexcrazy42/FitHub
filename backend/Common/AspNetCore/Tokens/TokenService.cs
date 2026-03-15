using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FitHub.Common.AspNetCore.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FitHub.Common.AspNetCore.Tokens;

internal sealed class TokenService : ITokenService
{
    private readonly IAuthOptions authOptions;

    public TokenService(IOptions<IAuthOptions> authOptions)
    {
        this.authOptions = authOptions.Value;
    }

    // Использование этой имплементации не предполагает какую-либо валидацию токена с помощью собственного сервиса
    public ClaimsPrincipal Validate(string token) => throw new NotImplementedException();

    public string Create(IReadOnlyList<Claim> claims)
    {
        var token = new JwtSecurityToken(
            issuer: authOptions.RequiredIssuer,
            claims: claims,
            expires: (DateTime.UtcNow + authOptions.RequiredCookieExpiration).ToUniversalTime(),
            signingCredentials: new SigningCredentials(
                SigningKey.GetSymmetricSecurityKey(
                    authOptions.RequiredSecretKey),
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

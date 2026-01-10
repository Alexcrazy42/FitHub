using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FitHub.Common.AspNetCore.Tokens;
using Microsoft.IdentityModel.Tokens;


namespace FitHub.Common.Testing;

internal sealed class TestTokenService : ITokenService
{
    // Применяется только для тестов - настоящий адрес не нужен
    private const string Issuer = "TestAuthentication";

    private static readonly SecurityKey SigningKey =
        AspNetCore.Tokens.SigningKey.GetSymmetricSecurityKey("some-very-test-test-test-test-test-test-secret-key");

    private readonly JwtSecurityTokenHandler tokenHandler;

    public TestTokenService()
    {
        tokenHandler = new JwtSecurityTokenHandler();
    }

    public ClaimsPrincipal Validate(string token)
    {
        var parameters = TokenDefaults.CreateTokenValidationParameters(Issuer, SigningKey);
        return tokenHandler.ValidateToken(token, parameters, out _);
    }

    public string Create(IReadOnlyList<Claim> claims)
    {
        var token = new JwtSecurityToken(
            issuer: Issuer,
            claims: claims,
            expires: DateTime.MaxValue,
            signingCredentials: new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

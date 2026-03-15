using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace FitHub.Common.AspNetCore.Tokens;

public static class TokenDefaults
{
    public static TokenValidationParameters CreateTokenValidationParameters(string issuer, SecurityKey signingKey)
    {
        return new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.NameIdentifier,

            // В нашем случае Audience излишен, он требуется только при сложных схемах авторизации
            ValidateAudience = false,

            // Требуем подпись у токена
            RequireSignedTokens = true,

            // Проверка издателя токена
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateIssuerSigningKey = true,

            // Требуем срок годности токена
            RequireExpirationTime = true,

            IssuerSigningKey = signingKey,

            // Валидировать срок годности и время активации токена
            ValidateLifetime = true,
        };
    }
}

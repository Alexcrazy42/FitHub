using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Users;

public class Token : IEntity<TokenId>, IAuditableEntity
{
    private User? user;

    private Token(TokenId id, IdentityUserId userId, string tokenString, TokenType tokenType, DateTimeOffset expiresOn)
    {
        Id = id;
        UserId = userId;
        TokenString = tokenString;
        TokenType = tokenType;
        ExpiresOn = expiresOn;
    }

    public TokenId Id { get; private set; }

    public IdentityUserId UserId { get; private set; }

    public User User
    {
        get => UnexpectedException.ThrowIfNull(user, "Пользователь неожиданно оказался null");
        private set => user = value;
    }

    public string TokenString { get; private set; }

    public TokenType TokenType { get; private set; }

    public DateTimeOffset ExpiresOn { get; private set; }

    public DateTimeOffset? AppliedAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public void SetCreatedAt(DateTimeOffset date) => CreatedAt = date;

    public void SetUpdatedAt(DateTimeOffset date) => UpdatedAt = date;

    public void SetAppliedAt(DateTimeOffset date) => AppliedAt = date;

    private static string GenerateTokenString()
    {
        return Guid.NewGuid().ToString();
    }

    private static DateTimeOffset GetExpiresOn(TokenType tokenType)
    {
        return tokenType switch
        {
            TokenType.ConfirmEmail => DateTimeOffset.UtcNow.AddDays(5),
            TokenType.ResetPassword => DateTimeOffset.UtcNow.AddMinutes(15),
            _ => throw new UnexpectedException($"Непредвиденный TokenType: {tokenType}")
        };
    }

    public static Token Create(User user, TokenType tokenType)
    {
        return new Token(TokenId.New(), user.Id, GenerateTokenString(), tokenType, GetExpiresOn(tokenType));
    }
}

using FitHub.Common.Entities;

namespace FitHub.Domain.Users;

public class UserRefreshToken : IEntity<UserRefreshTokenId>
{
    private User? user;

    public UserRefreshToken(UserRefreshTokenId id, string refreshToken, DateTimeOffset expiresAt, UserId userId)
    {
        Id = id;
        RefreshToken = refreshToken;
        UserId = userId;
    }

    public UserRefreshTokenId Id { get; }

    public string RefreshToken { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }

    public UserId UserId { get; private set; }

    public User User
    {
        get => user ?? throw new UnexpectedException("Пользователь неожиданно оказался null");
        private set => user = value;
    }
}

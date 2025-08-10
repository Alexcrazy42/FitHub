using FitHub.Common.Entities;
using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Users;

public class User : IEntity<UserId>, IAuditableEntity
{
    private List<UserRefreshToken> refreshTokens = [];

    private User(UserId id, string email, string surname, string name, string passwordHash)
    {
        Id = id;
        Email = email;
        Surname = surname;
        Name = name;
        PasswordHash = passwordHash;
    }

    public UserId Id { get; }

    public string Email { get; private set; }

    public string PasswordHash { get; private set; }

    public UserType Type { get; private set; }

    public string Surname { get; private set; }

    public string Name { get; private set; }

    public bool IsVerified { get; private set; }

    public DateTimeOffset LastSeenAt { get; private set; }

    public bool IsOnline { get; private set; }

    public IReadOnlyList<UserRefreshToken> RefreshTokens => refreshTokens;

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public void SetType(UserType type)
    {
        Type = type;
    }

    public void SetCreatedAt(DateTimeOffset date)
    {
        CreatedAt = date;
    }

    public void SetUpdatedAt(DateTimeOffset date)
    {
        UpdatedAt = date;
    }

    public void SetIsVerified(bool isVerified)
    {
        IsVerified = isVerified;
    }

    public void SetLastSeenAt(DateTimeOffset date)
    {
        LastSeenAt = date;
    }

    public void SetIsOnline(bool isOnline)
    {
        IsOnline = isOnline;
    }

    public void AddRefreshToken(UserRefreshToken refreshToken)
    {
        refreshTokens.Add(refreshToken);
    }
}

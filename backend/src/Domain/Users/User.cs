using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Users;

public class User : IdentityUser, IAuditableEntity
{

    private User(IdentityUserId id, string surname, string name, string email, string nickname, string passwordHash) : base(id, nickname, email, passwordHash)
    {
        Id = id;
        Surname = surname;
        Name = name;
        Email = email;
        Nickname = nickname;
        PasswordHash = passwordHash;
        LastSeenAt = DateTimeOffset.Now;
    }

    public UserType Type { get; private set; }

    public string Surname { get; private set; }

    public string Name { get; private set; }

    public bool IsVerified { get; private set; }

    public DateTimeOffset LastSeenAt { get; private set; }

    public bool IsOnline { get; private set; }

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
}

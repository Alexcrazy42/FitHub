using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Users;

public class User : IdentityUser, IAuditableEntity
{
    private User(IdentityUserId id, string surname, string name, string email, string passwordHash, IdentityUserType userType, DateTimeOffset startRegistrationAt, DateTimeOffset lastSeenAt)
        : base(id, email, passwordHash, userType, startRegistrationAt)
    {
        Id = id;
        Surname = surname;
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        UserType = userType;
        LastSeenAt = lastSeenAt;
    }

    public string Surname { get; private set; }

    public string Name { get; private set; }

    public DateTimeOffset LastSeenAt { get; private set; }

    public bool IsOnline { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public void SetCreatedAt(DateTimeOffset date)
    {
        CreatedAt = date;
    }

    public void SetUpdatedAt(DateTimeOffset date)
    {
        UpdatedAt = date;
    }

    public static User Create(IdentityUserId id,
        string surname,
        string name,
        string email,
        string passwordHash,
        IdentityUserType userType,
        DateTimeOffset startRegistrationAt,
        DateTimeOffset lastSeenAt,
        DateTimeOffset createdAt)
    {
        return new User(id, surname, name, email, passwordHash, userType, startRegistrationAt, lastSeenAt)
        {
            IsEmailConfirmed = true,
            IsTemporaryPassword = false,
            IsActive = true,
            CreatedAt = createdAt,
            UpdatedAt = createdAt,
        };
    }

    public static User Create(string surname, string name, string email, string passwordHash, IdentityUserType userType, DateTimeOffset startRegistrationAt, DateTimeOffset lastSeenAt)
    {
        return new User(IdentityUserId.New(), surname, name, email, passwordHash, userType, startRegistrationAt, lastSeenAt);
    }
}

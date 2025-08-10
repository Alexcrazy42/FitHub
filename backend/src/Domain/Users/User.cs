using FitHub.Common.Entities;
using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Users;

public class User : IEntity<UserId>, IAuditableEntity
{
    private User(UserId id, string email, string surname, string name)
    {
        Id = id;
        Email = email;
        Surname = surname;
        Name = name;
    }

    public UserId Id { get; }

    public string Email { get; private set; }

    public UserType Type { get; private set; }

    public string Surname { get; private set; }

    public string Name { get; private set; }

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
}

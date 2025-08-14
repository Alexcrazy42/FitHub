using FitHub.Common.Entities;

namespace FitHub.Common.AspNetCore.Accounting;

public class IdentityUser : IEntity<IdentityUserId>
{
    public IdentityUser(IdentityUserId id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }

    public IdentityUserId Id { get; protected set; }

    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string Name { get; protected set; }

    public string Email { get; protected set; }
}

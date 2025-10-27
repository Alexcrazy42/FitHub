using FitHub.Common.Entities;

namespace FitHub.Common.AspNetCore.Accounting;

public class IdentityUser : IEntity<IdentityUserId>
{
    public IdentityUser(IdentityUserId id, string nickname, string email, string passwordHash, IdentityUserType userType)
    {
        Id = id;
        Nickname = nickname;
        Email = email;
        PasswordHash = passwordHash;
        UserType = userType;
    }

    public IdentityUserId Id { get; protected set; }

    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string Nickname { get; protected set; }

    /// <summary>
    /// Почта
    /// </summary>
    public string Email { get; protected set; }

    /// <summary>
    /// Пароль
    /// </summary>
    public string PasswordHash { get; protected set; }

    public IdentityUserType UserType { get; protected set; }
}

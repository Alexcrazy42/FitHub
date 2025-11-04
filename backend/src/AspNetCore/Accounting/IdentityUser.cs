using FitHub.Common.Entities;

namespace FitHub.Common.AspNetCore.Accounting;

public class IdentityUser : IEntity<IdentityUserId>
{
    public IdentityUser(IdentityUserId id, string email, string passwordHash, IdentityUserType userType, DateTimeOffset startRegistrationAt)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        UserType = userType;
        IsEmailConfirmed = false;
        IsTemporaryPassword = true;
        IsActive = false;
        StartRegistrationAt = startRegistrationAt;
    }

    public IdentityUserId Id { get; protected set; }

    /// <summary>
    /// Почта
    /// </summary>
    public string Email { get; protected set; }

    /// <summary>
    /// Пароль
    /// </summary>
    public string PasswordHash { get; protected set; }

    public IdentityUserType UserType { get; protected set; }

    public bool IsEmailConfirmed { get; protected set; }

    public bool IsTemporaryPassword { get; protected set; }

    public bool IsActive { get; protected set; }

    public DateTimeOffset StartRegistrationAt { get; protected set; }

    public DateTimeOffset? StartActiveAt { get; protected set; }


    public void SetEmailConfirmed(bool value)
    {
        IsEmailConfirmed = value;
    }

    public void SetPassword(string passwordHash)
    {
        IsTemporaryPassword = false;
        PasswordHash = passwordHash;
    }

    public void SetActive(bool value)
    {
        IsActive = value;
    }

    public bool IsLoginFlowDone()
    {
        return IsEmailConfirmed && !IsTemporaryPassword && IsActive;
    }
}

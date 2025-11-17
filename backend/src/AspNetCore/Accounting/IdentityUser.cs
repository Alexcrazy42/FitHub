using FitHub.Common.Entities;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

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
        if (StartActiveAt is null)
        {
            throw new ValidationException("Невозможно поменять статус для неактивированного пользователя!");
        }
        IsActive = value;
    }

    public void SetActiveAt(DateTimeOffset value)
    {
        StartActiveAt = value;
    }

    public bool IsLoginFlowDone()
    {
        return IsEmailConfirmed && !IsTemporaryPassword && IsActive;
    }
}

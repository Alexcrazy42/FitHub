using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Users;

public class Session : IEntity<SessionId>, IAuditableEntity
{
    private User? user;

    public Session(SessionId id, IdentityUserId userId, bool isActive, DateTimeOffset expiresOn)
    {
        Id = id;
        UserId = userId;
        IsActive = isActive;
        ExpiresOn = expiresOn;
    }

    public SessionId Id { get; private set; }

    public IdentityUserId UserId { get; private set; }

    public User User
    {
        get => UnexpectedException.ThrowIfNull(user, "");
        private set => user = value;
    }

    public bool IsActive { get; private set; }

    public DateTimeOffset ExpiresOn { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }


    public void SetActive(bool isActive)
    {
        IsActive = isActive;
    }

    public static Session Create(User user, bool isActive, DateTimeOffset expiresOn)
    {
        return new Session(SessionId.New(), user.Id, isActive, expiresOn);
    }
}

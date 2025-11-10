using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Identity;
using FitHub.Domain.Trainings;

namespace FitHub.Domain.Users;

public class Visitor : IEntity<VisitorId>, IAuditableEntity
{
    private List<GroupTraining> groupTrainings = [];
    private List<PersonalTraining> personalTrainings = [];
    private User? user;

    public Visitor(VisitorId id, IdentityUserId userId)
    {
        Id = id;
        UserId = userId;
    }

    public VisitorId Id { get; }

    public IdentityUserId UserId { get; private set; }

    public User User
    {
        get => UnexpectedException.ThrowIfNull(user, "Пользователь неожиданно оказался null");
        private set => user = value;
    }

    public IReadOnlyList<GroupTraining> GroupTrainings => groupTrainings;

    public IReadOnlyList<PersonalTraining> PersonalTrainings => personalTrainings;

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

    public static Visitor Create(User user)
    {
        return new Visitor(VisitorId.New(), user.Id);
    }
}

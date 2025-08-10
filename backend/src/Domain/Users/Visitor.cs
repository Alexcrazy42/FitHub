using FitHub.Common.Entities;
using FitHub.Common.Entities.Identity;
using FitHub.Domain.Trainings;

namespace FitHub.Domain.Users;

public class Visitor : IEntity<VisitorId>, IAuditableEntity
{
    private List<GroupTraining> groupTrainings = [];
    private List<PersonalTraining> personalTrainings = [];

    public Visitor(VisitorId id)
    {
        Id = id;
    }

    public VisitorId Id { get; }

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
}

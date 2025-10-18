using FitHub.Common.Entities;
using FitHub.Domain.Equipments;

namespace FitHub.Domain.Trainings;

public class MuscleGroup : IEntity<MuscleGroupId>
{
    private MuscleGroup(MuscleGroupId id, string name)
    {
        Id = id;
        Name = name;
    }

    public MuscleGroupId Id { get; }

    public string Name { get; private set; }

    public MuscleGroupId? ParentId { get; private set; }

    public MuscleGroup? Parent { get; private set; }

    public void SetParent(MuscleGroup parent)
    {
        Parent = parent;
    }
}

using FitHub.Common.Entities;
using FitHub.Domain.Equipments;

namespace FitHub.Domain.Trainings;

public class MuscleGroup : IEntity<MuscleGroupId>
{
    private List<MuscleGroup> childrens = [];

    private MuscleGroup(MuscleGroupId id, string name)
    {
        Id = id;
        Name = name;
    }

    public MuscleGroupId Id { get; }

    public string Name { get; private set; }

    public MuscleGroupId? ParentId { get; private set; }

    public MuscleGroup? Parent { get; private set; }

    public IReadOnlyList<MuscleGroup> Childrens => childrens;

    public void SetName(string name) => Name = name;

    public void SetParent(MuscleGroup parent)
    {
        Parent = parent;
        ParentId = parent.Id;
    }

    public static MuscleGroup Create(string name, MuscleGroup? parent)
    {
        return new MuscleGroup(MuscleGroupId.New(), name)
        {
            ParentId = parent?.Id,
            Parent = parent
        };
    }
}

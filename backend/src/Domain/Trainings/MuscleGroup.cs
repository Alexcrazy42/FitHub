using FitHub.Common.Entities;

namespace FitHub.Domain.Trainings;

public class MuscleGroup : IEntity<MuscleGroupId>
{
    private MuscleGroup(MuscleGroupId id, string name, string imageUrl)
    {
        Id = id;
        Name = name;
        ImageUrl = imageUrl;
    }

    public MuscleGroupId Id { get; }

    public string Name { get; private set; }

    public string ImageUrl { get; private set; }

    public MuscleGroupId? ParentId { get; private set; }

    public MuscleGroup? Parent { get; private set; }

    public void SetImageUrl(string imageUrl)
    {
        ImageUrl = imageUrl;
    }

    public void SetParent(MuscleGroup parent)
    {
        Parent = parent;
    }
}

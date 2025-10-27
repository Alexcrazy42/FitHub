using FitHub.Common.Entities;

namespace FitHub.Domain.Trainings;

public class TrainingType : IEntity<TrainingTypeId>
{
    private TrainingType(TrainingTypeId id, string name)
    {
        Id = id;
        Name = name;
    }

    public TrainingTypeId Id { get; }

    public string Name { get; private set; }

    public void SetName(string name)
    {
        Name = name;
    }

    public static TrainingType Create(string name)
    {
        return new TrainingType(TrainingTypeId.New(), name);
    }
}

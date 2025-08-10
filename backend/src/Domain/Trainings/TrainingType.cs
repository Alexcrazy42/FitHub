using FitHub.Common.Entities;

namespace FitHub.Domain.Trainings;

public class TrainingType : IEntity<TrainingTypeId>
{
    public TrainingType(TrainingTypeId id, string name)
    {
        Id = id;
        Name = name;
    }

    public TrainingTypeId Id { get; }

    public string Name { get; private set; }
}

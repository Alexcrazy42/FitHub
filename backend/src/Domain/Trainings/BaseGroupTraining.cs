using FitHub.Common.Entities;

namespace FitHub.Domain.Trainings;

public class BaseGroupTraining : IEntity<BaseGroupTrainingId>
{
    private const int MinComplexityValue = 1;
    private const int MaxComplexityValue = 3;

    public BaseGroupTraining(BaseGroupTrainingId id, string name, string description, int durationInMinutes, int complexity)
    {
        Id = id;
        Name = name;
        Description = description;
        DurationInMinutes = durationInMinutes;
        Complexity = complexity;
    }

    public BaseGroupTrainingId Id { get; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public int DurationInMinutes { get; private set; }

    public int Complexity { get; private set; }

    public TrainingTypeId? TypeId { get; private set; }

    public TrainingType? Type { get; }


    public void SetComplexity(int complexity)
    {
        if (complexity < MinComplexityValue || complexity > MaxComplexityValue)
        {
            throw new CommonException($"Сложность тренировки должна быть от {MinComplexityValue} до {MaxComplexityValue}");
        }
        Complexity = complexity;
    }
}

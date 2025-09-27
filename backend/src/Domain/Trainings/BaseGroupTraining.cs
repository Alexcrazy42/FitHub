using FitHub.Common.Entities;

namespace FitHub.Domain.Trainings;

public class BaseGroupTraining : IEntity<BaseGroupTrainingId>
{
    private const int MinComplexityValue = 1;
    private const int MaxComplexityValue = 3;
    private const int MinNameLength = 10;

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


    public void SetName(string name)
    {
        ValidateName(name);
        Name = name;
    }

    public static void ValidateName(string? name)
    {
        if (name is null)
        {
            throw new ValidationException("Имя не может быть пустым");
        }

        if (name.Length < MinNameLength)
        {
            throw new ValidationException($"Длина имени не может быть меньше {MinNameLength}");
        }
    }

    public void SetComplexity(int complexity)
    {
        ValidateComplexity(complexity);
        Complexity = complexity;
    }

    public static void ValidateComplexity(int? complexity)
    {
        if (complexity < MinComplexityValue || complexity > MaxComplexityValue)
        {
            throw new CommonException($"Сложность тренировки должна быть от {MinComplexityValue} до {MaxComplexityValue}");
        }
    }
}

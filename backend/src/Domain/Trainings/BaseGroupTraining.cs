using FitHub.Common.Entities;

namespace FitHub.Domain.Trainings;

public class BaseGroupTraining : IEntity<BaseGroupTrainingId>
{
    private const int MinComplexityValue = 1;
    private const int MaxComplexityValue = 3;
    private const int MinNameLength = 10;
    private List<TrainingType> trainingTypes = [];

    private BaseGroupTraining(BaseGroupTrainingId id, string name, string description, int durationInMinutes, int complexity, bool isActive)
    {
        Id = id;
        Name = name;
        Description = description;
        DurationInMinutes = durationInMinutes;
        Complexity = complexity;
        IsActive = isActive;
    }

    public BaseGroupTrainingId Id { get; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public int DurationInMinutes { get; private set; }

    public int Complexity { get; private set; }

    public bool IsActive { get; private set; }

    public bool IsDeleted { get; private set; }

    public IReadOnlyList<TrainingType> TrainingTypes => trainingTypes;

    public void SetTrainingTypes(List<TrainingType> newTrainingTypes)
        => trainingTypes = newTrainingTypes;

    public void SetName(string name)
    {
        ValidateName(name);
        Name = name;
    }

    public void SetDescription(string description)
    {
        Description = description;
    }

    public void SetDurationInMinutes(int durationInMinutes)
    {
        DurationInMinutes = durationInMinutes;
    }

    public void SetIsActive(bool isActive)
    {
        IsActive = isActive;
    }

    public void SetIsDeleted(bool isDeleted)
    {
        IsDeleted = isDeleted;
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

    public static BaseGroupTraining Create(string name, string description, int durationInMinutes, int complexity, bool isActive)
    {
        ValidateName(name);
        ValidateComplexity(complexity);
        return new BaseGroupTraining(BaseGroupTrainingId.New(), name, description, durationInMinutes, complexity, isActive)
        {
            IsDeleted = false
        };
    }
}

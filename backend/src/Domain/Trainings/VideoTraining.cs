using FitHub.Common.Entities;

namespace FitHub.Domain.Trainings;

public class VideoTraining : IEntity<VideoTrainingId>
{
    private const int MinComplexityValue = 1;
    private const int MaxComplexityValue = 3;

    private TrainingType? trainingType;
    private readonly List<MuscleGroup> muscleGroups = [];

    public VideoTraining(VideoTrainingId id, string name, string description, int complexity, int durationInMinutes, TrainingTypeId trainingTypeId)
    {
        Id = id;
        Name = name;
        Description = description;
        TrainingTypeId = trainingTypeId;
        Complexity = complexity;
        DurationInMinutes = durationInMinutes;
    }

    public VideoTrainingId Id { get; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public int Complexity { get; private set; }

    public int DurationInMinutes { get; private set; }

    public string? VideoUrl { get; private set; }

    public TrainingTypeId TrainingTypeId { get; private set; }

    public IReadOnlyList<MuscleGroup> MuscleGroups => muscleGroups;

    public TrainingType TrainingType
    {
        get => UnexpectedException.ThrowIfNull(trainingType, "Тип тренировки неожиданно оказался null");
        private set => trainingType = value;
    }

    public void SetComplexity(int complexity)
    {
        if (complexity < MinComplexityValue || complexity > MaxComplexityValue)
        {
            throw new CommonException($"Сложность тренировки должна быть от {MinComplexityValue} до {MaxComplexityValue}");
        }

        Complexity = complexity;
    }

    public void SetVideoUrl(string videoUrl)
    {
        VideoUrl = videoUrl;
    }
}

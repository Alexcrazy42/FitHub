using FitHub.Contracts.V1.Trainings.TrainingTypes;

namespace FitHub.Contracts.V1.Trainings.BaseGroupTrainings;

public sealed class BaseGroupTrainingResponse
{
    public Guid? Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int? DurationInMinutes { get; set; }

    public int? Complexity { get; set; }

    public bool? IsActive { get; set; }

    public List<TrainingTypeResponse> TrainingTypes { get; set; } = [];
}

namespace FitHub.Contracts.V1.Trainings.BaseGroupTrainings;

public sealed class CreateBaseGroupTrainingRequest
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public int? DurationInMinutes { get; set; }

    public int? Complexity { get; set; }

    public bool? IsActive { get; set; }

    public List<string> TrainingTypeIds { get; set; } = [];
}

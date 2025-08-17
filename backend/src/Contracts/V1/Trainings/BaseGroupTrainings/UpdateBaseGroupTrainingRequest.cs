namespace FitHub.Contracts.V1.Trainings.BaseGroupTrainings;

public class UpdateBaseGroupTrainingRequest
{
    public Guid? Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int? DurationInMinutes { get; set; }

    public int? Complexity { get; set; }

    public Guid? TrainingTypeId { get; set; }
}

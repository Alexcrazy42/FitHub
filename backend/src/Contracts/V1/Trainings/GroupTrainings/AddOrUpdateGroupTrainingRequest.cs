namespace FitHub.Contracts.V1.Trainings.GroupTrainings;

public class AddOrUpdateGroupTrainingRequest
{
    public string? BaseGroupTrainingId { get; set; }

    public string? GymId { get; set; }

    public string? TrainerId { get; set; }

    public DateTimeOffset? StartTime { get; set; }

    public DateTimeOffset? EndTime { get; set; }

    public bool? IsActive { get; set; }
}

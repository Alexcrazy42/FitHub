namespace FitHub.Contracts.V1.Trainings.GroupTrainings;

public class GroupTrainingSearchRequest
{
    public string? GymId { get; set; }

    public string? TrainerId { get; set; }

    public DateTimeOffset? StartTime { get; set; }

    public DateTimeOffset? EndTime { get; set; }
}

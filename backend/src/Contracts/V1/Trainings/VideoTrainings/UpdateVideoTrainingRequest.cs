namespace FitHub.Contracts.V1.Trainings.VideoTrainings;

public class UpdateVideoTrainingRequest
{
    public Guid? Id { get; set;  }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int? Complexity { get; set; }

    public int? DurationInMinutes { get; set; }

    public string? VideoUrl { get; set; }

    public Guid? TrainingTypeId { get; set; }

    public IReadOnlyList<Guid?> MuscleGroupsIds = [];
}

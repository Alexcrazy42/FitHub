using FitHub.Contracts.V1.Equipments.MuscleGroups;
using FitHub.Contracts.V1.Trainings.TrainingTypes;

namespace FitHub.Contracts.V1.Trainings.VideoTrainings;

public sealed class VideoTrainingResponse
{
    public Guid? Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int? Complexity { get; set; }

    public int? DurationInMinutes { get; set; }

    public string? VideoUrl { get; set; }

    public TrainingTypeResponse? TrainingType { get; set; }

    public IReadOnlyList<MuscleGroupResponse> MuscleGroups = [];
}

using FitHub.Contracts.V1.Equipments.Gyms;
using FitHub.Contracts.V1.Trainings.BaseGroupTrainings;
using FitHub.Contracts.V1.Users.Trainers;
using FitHub.Contracts.V1.Users.Visitors;

namespace FitHub.Contracts.V1.Trainings.GroupTrainings;

public sealed class GroupTrainingResponse
{
    public string? Id { get; set; }

    public BaseGroupTrainingResponse? BaseGroupTraining { get; set; }

    public GymResponse? Gym { get; set; }

    public TrainerResponse? Trainer { get; set; }

    public IReadOnlyList<VisitorResponse> Participants { get; set; } = [];

    public DateTimeOffset? StartTime { get; set; }

    public DateTimeOffset? EndTime { get; set; }

    public bool? IsActive { get; set; }
}

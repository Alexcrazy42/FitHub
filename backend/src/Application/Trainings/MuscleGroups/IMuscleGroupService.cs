using FitHub.Contracts.V1.Equipments.MuscleGroups;
using FitHub.Domain.Trainings;

namespace FitHub.Application.Trainings.MuscleGroups;

public interface IMuscleGroupService
{
    Task<MuscleGroup> CreateMuscleGroupAsync(CreateMuscleGroupRequest request, CancellationToken ct = default);

    Task<MuscleGroup> UpdateMuscleGroupAsync(UpdateMuscleGroupRequest request, CancellationToken ct = default);

    Task DeleteMuscleGroupAsync(MuscleGroupId id, CancellationToken ct = default);
}

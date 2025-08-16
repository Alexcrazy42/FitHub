using FitHub.Contracts.V1.Equipments.MuscleGroups;
using FitHub.Domain.Trainings;

namespace FitHub.Application.Trainings.MuscleGroups;

public class MuscleGroupService : IMuscleGroupService
{
    private readonly IMuscleGroupRepository muscleGroupRepository;

    public MuscleGroupService(IMuscleGroupRepository muscleGroupRepository)
    {
        this.muscleGroupRepository = muscleGroupRepository;
    }

    public Task<MuscleGroup> CreateMuscleGroupAsync(CreateMuscleGroupRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<MuscleGroup> UpdateMuscleGroupAsync(UpdateMuscleGroupRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task DeleteMuscleGroupAsync(MuscleGroupId id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}

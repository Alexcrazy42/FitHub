using FitHub.Application.Common;
using FitHub.Contracts.V1.Equipments.MuscleGroups;
using FitHub.Domain.Trainings;

namespace FitHub.Application.Trainings.MuscleGroups;

public interface IMuscleGroupService
{
    Task<PagedResult<MuscleGroup>> GetAll(PagedQuery pagedQuery, CancellationToken ct);

    Task<IReadOnlyList<MuscleGroup>> GetByIds(List<MuscleGroupId> ids, CancellationToken ct);


    Task<MuscleGroup> GetById(MuscleGroupId id, CancellationToken ct);
    Task<MuscleGroup> CreateMuscleGroupAsync(CreateMuscleGroupRequest request, CancellationToken ct = default);

    Task<MuscleGroup> UpdateMuscleGroupAsync(UpdateMuscleGroupRequest request, CancellationToken ct = default);

    Task DeleteMuscleGroupAsync(MuscleGroupId id, CancellationToken ct = default);
}

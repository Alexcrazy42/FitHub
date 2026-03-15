using FitHub.Application.Common;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Trainings;

namespace FitHub.Application.Trainings.MuscleGroups;

public interface IMuscleGroupRepository : IPendingRepository<MuscleGroup, MuscleGroupId>
{
    Task<PagedResult<MuscleGroup>> GetAll(PagedQuery pagedQuery, CancellationToken ct);

    Task<MuscleGroup> GetById(MuscleGroupId id, CancellationToken ct);
}

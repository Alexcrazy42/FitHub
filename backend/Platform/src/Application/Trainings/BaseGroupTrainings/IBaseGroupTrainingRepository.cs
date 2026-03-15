using FitHub.Application.Common;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Trainings;

namespace FitHub.Application.Trainings.BaseGroupTrainings;

public interface IBaseGroupTrainingRepository : IPendingRepository<BaseGroupTraining, BaseGroupTrainingId>
{
    Task<PagedResult<BaseGroupTraining>> GetAsync(PagedQuery query, CancellationToken ct);
    Task<BaseGroupTraining> GetById(BaseGroupTrainingId id, CancellationToken ct);
}

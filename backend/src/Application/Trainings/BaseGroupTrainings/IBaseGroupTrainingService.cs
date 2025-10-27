using FitHub.Application.Common;
using FitHub.Contracts.V1.Trainings.BaseGroupTrainings;
using FitHub.Domain.Trainings;

namespace FitHub.Application.Trainings.BaseGroupTrainings;

public interface IBaseGroupTrainingService
{
    Task<PagedResult<BaseGroupTraining>> GetAsync(PagedQuery pagedQuery, CancellationToken ct = default);

    Task<BaseGroupTraining> GetByIdAsync(BaseGroupTrainingId id, CancellationToken ct = default);

    Task<BaseGroupTraining> CreateAsync(CreateBaseGroupTrainingRequest request, CancellationToken ct = default);

    Task<BaseGroupTraining> UpdateAsync(BaseGroupTrainingId id, CreateBaseGroupTrainingRequest request, CancellationToken ct = default);

    Task DeleteAsync(BaseGroupTrainingId id, CancellationToken ct = default);
}

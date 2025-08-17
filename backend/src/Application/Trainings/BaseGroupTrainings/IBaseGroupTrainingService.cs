using FitHub.Contracts.V1.Trainings.BaseGroupTrainings;
using FitHub.Domain.Trainings;

namespace FitHub.Application.Trainings.BaseGroupTrainings;

public interface IBaseGroupTrainingService
{
    Task<BaseGroupTraining> CreateAsync(CreateBaseGroupTrainingRequest baseGroupTraining, CancellationToken ct = default);

    Task<BaseGroupTraining> UpdateAsync(UpdateBaseGroupTrainingRequest baseGroupTraining, CancellationToken ct = default);

    Task DeleteAsync(BaseGroupTrainingId id, CancellationToken ct = default);
}

using FitHub.Contracts.V1.Trainings.BaseGroupTrainings;
using FitHub.Domain.Trainings;

namespace FitHub.Application.Trainings.BaseGroupTrainings;

public class BaseGroupTrainingService : IBaseGroupTrainingService
{
    private readonly IBaseGroupTrainingRepository baseGroupTrainingRepository;

    public BaseGroupTrainingService(IBaseGroupTrainingRepository baseGroupTrainingRepository)
    {
        this.baseGroupTrainingRepository = baseGroupTrainingRepository;
    }

    public Task<BaseGroupTraining> CreateAsync(CreateBaseGroupTrainingRequest baseGroupTraining, CancellationToken ct) => throw new NotImplementedException();

    public Task<BaseGroupTraining> UpdateAsync(UpdateBaseGroupTrainingRequest baseGroupTraining, CancellationToken ct) => throw new NotImplementedException();

    public Task DeleteAsync(BaseGroupTrainingId id, CancellationToken ct) => throw new NotImplementedException();
}

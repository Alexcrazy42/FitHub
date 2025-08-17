using FitHub.Contracts.V1.Trainings.TrainingTypes;
using FitHub.Domain.Trainings;

namespace FitHub.Application.Trainings.TrainingTypes;

public class TrainingTypeService : ITrainingTypeService
{
    private readonly ITrainingTypeRepository trainingTypeRepository;

    public TrainingTypeService(ITrainingTypeRepository trainingTypeRepository)
    {
        this.trainingTypeRepository = trainingTypeRepository;
    }

    public Task<TrainingType> CreateAsync(CreateTrainingTypeRequest request, CancellationToken ct) => throw new NotImplementedException();

    public Task<TrainingType> UpdateAsync(UpdateTrainingTypeRequest request, CancellationToken ct) => throw new NotImplementedException();

    public Task DeleteAsync(TrainingTypeId id, CancellationToken ct) => throw new NotImplementedException();
}

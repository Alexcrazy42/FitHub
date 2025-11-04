using FitHub.Contracts.V1.Trainings.TrainingTypes;
using FitHub.Domain.Trainings;

namespace FitHub.Application.Trainings.TrainingTypes;

public interface ITrainingTypeService
{
    Task<TrainingType> CreateAsync(CreateTrainingTypeRequest request, CancellationToken ct = default);

    Task<TrainingType> UpdateAsync(TrainingTypeId id, CreateTrainingTypeRequest request, CancellationToken ct = default);

    Task DeleteAsync(TrainingTypeId id, CancellationToken ct = default);
}

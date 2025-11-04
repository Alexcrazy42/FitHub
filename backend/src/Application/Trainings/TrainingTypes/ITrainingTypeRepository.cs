using FitHub.Common.Entities.Storage;
using FitHub.Domain.Trainings;

namespace FitHub.Application.Trainings.TrainingTypes;

public interface ITrainingTypeRepository : IPendingRepository<TrainingType, TrainingTypeId>
{
    Task<IReadOnlyList<TrainingType>> GetAsync(IReadOnlyList<TrainingTypeId> ids, CancellationToken ct);
}

using FitHub.Common.Entities.Storage;
using FitHub.Domain.Trainings;

namespace FitHub.Application.Trainings.TrainingTypes;

public interface ITrainingTypeRepository : IPendingRepository<TrainingType, TrainingTypeId>
{

}

using FitHub.Application.Trainings.TrainingTypes;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Trainings;

namespace FitHub.Data.Trainings.TrainingTypes;

public class TrainingTypeRepository : DefaultPendingRepository<TrainingType, TrainingTypeId, DataContext>, ITrainingTypeRepository
{
    private readonly DataContext context;

    public TrainingTypeRepository(DataContext context) : base(context)
    {
        this.context = context;
    }
}

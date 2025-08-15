using FitHub.Application.Trainings.BaseGroupTrainings;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Trainings;

namespace FitHub.Data.Trainings.BaseGroupTrainings;

public class BaseGroupTrainingRepository : DefaultPendingRepository<BaseGroupTraining, BaseGroupTrainingId, DataContext>, IBaseGroupTrainingRepository
{
    public BaseGroupTrainingRepository(DataContext context) : base(context)
    {
    }
}

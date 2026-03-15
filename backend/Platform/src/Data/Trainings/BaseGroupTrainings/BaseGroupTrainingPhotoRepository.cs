using FitHub.Application.Trainings.BaseGroupTrainings;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Trainings;

namespace FitHub.Data.Trainings.BaseGroupTrainings;

public class BaseGroupTrainingPhotoRepository : DefaultPendingRepository<BaseGroupTrainingPhoto, BaseGroupTrainingPhotoId, DataContext>, IBaseGroupTrainingPhotoRepository
{
    public BaseGroupTrainingPhotoRepository(DataContext context) : base(context)
    {
    }
}

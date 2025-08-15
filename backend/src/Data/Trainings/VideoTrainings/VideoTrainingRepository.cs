using FitHub.Application.Trainings.VideoTrainings;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Trainings;

namespace FitHub.Data.Trainings.VideoTrainings;

public class VideoTrainingRepository : DefaultPendingRepository<VideoTraining, VideoTrainingId, DataContext>, IVideoTrainingRepository
{
    public VideoTrainingRepository(DataContext context) : base(context)
    {
    }
}

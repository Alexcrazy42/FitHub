using FitHub.Common.Entities.Storage;
using FitHub.Domain.Trainings;

namespace FitHub.Application.Trainings.VideoTrainings;

public interface IVideoTrainingRepository : IPendingRepository<VideoTraining, VideoTrainingId>
{

}

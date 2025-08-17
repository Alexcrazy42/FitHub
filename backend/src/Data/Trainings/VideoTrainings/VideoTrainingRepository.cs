using FitHub.Application.Trainings.VideoTrainings;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Trainings;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Trainings.VideoTrainings;

public class VideoTrainingRepository : DefaultPendingRepository<VideoTraining, VideoTrainingId, DataContext>, IVideoTrainingRepository
{
    private readonly DataContext context;

    public VideoTrainingRepository(DataContext context) : base(context)
    {
        this.context = context;
    }

    public Task<VideoTraining?> GetByIdAsync(VideoTrainingId id, CancellationToken ct)
    {
        return context.VideoTrainings
            .Include(x => x.TrainingType)
            .Include(x => x.MuscleGroups)
            .SingleOrDefaultAsync(x => x.Id == id, ct);
    }
}

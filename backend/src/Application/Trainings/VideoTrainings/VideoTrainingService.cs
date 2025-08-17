using FitHub.Contracts.V1.Trainings.VideoTrainings;
using FitHub.Domain.Trainings;

namespace FitHub.Application.Trainings.VideoTrainings;

public class VideoTrainingService : IVideoTrainingService
{
    private readonly IVideoTrainingRepository repository;

    public VideoTrainingService(IVideoTrainingRepository repository)
    {
        this.repository = repository;
    }

    public Task<VideoTraining> CreateAsync(CreateVideoTrainingRequest request, CancellationToken ct) => throw new NotImplementedException();

    public Task<VideoTraining> UpdateAsync(UpdateVideoTrainingRequest request, CancellationToken ct) => throw new NotImplementedException();

    public Task DeleteAsync(VideoTrainingId id, CancellationToken ct) => throw new NotImplementedException();
}

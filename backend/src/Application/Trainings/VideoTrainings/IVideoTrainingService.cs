using FitHub.Contracts.V1.Trainings.VideoTrainings;
using FitHub.Domain.Trainings;

namespace FitHub.Application.Trainings.VideoTrainings;

public interface IVideoTrainingService
{
    Task<VideoTraining> CreateAsync(CreateVideoTrainingRequest request, CancellationToken ct = default);

    Task<VideoTraining> UpdateAsync(UpdateVideoTrainingRequest request, CancellationToken ct = default);

    Task DeleteAsync(VideoTrainingId id, CancellationToken ct = default);
}

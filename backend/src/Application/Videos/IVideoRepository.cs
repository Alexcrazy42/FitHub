using FitHub.Common.Entities.Storage;
using FitHub.Domain.Videos;

namespace FitHub.Application.Videos;

public interface IVideoRepository : IPendingRepository<Video, VideoId>
{
    Task<IReadOnlyList<Video>> GetAllWithResolutionsAsync(CancellationToken ct);
    Task<Video?> GetWithResolutionsAsync(VideoId id, CancellationToken ct);
    Task PendingAddResolutionAsync(VideoResolution resolution, CancellationToken ct);
}

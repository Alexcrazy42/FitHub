using FitHub.Domain.Videos;

namespace FitHub.Application.Videos;

public interface IVideoEncodingQueue
{
    Task EnqueueAsync(VideoId id, CancellationToken ct);
}

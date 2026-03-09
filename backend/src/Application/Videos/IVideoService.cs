using FitHub.Domain.Videos;

namespace FitHub.Application.Videos;

public record VideoUploadInitResult(VideoId VideoId, string PresignedPutUrl);

public record VideoResolutionUrl(VideoQuality Quality, int WidthPx, int HeightPx, int BitrateKbps, string Url);

public interface IVideoService
{
    Task<VideoUploadInitResult> InitUploadAsync(string title, string fileExtension, CancellationToken ct);
    Task<Video> ConfirmUploadAsync(VideoId id, CancellationToken ct);
    Task<Video> GetAsync(VideoId id, CancellationToken ct);
    Task<IReadOnlyList<Video>> GetAllAsync(CancellationToken ct);
    Task<IReadOnlyList<VideoResolutionUrl>> GetResolutionUrlsAsync(VideoId id, CancellationToken ct);
    Task DeleteAsync(VideoId id, CancellationToken ct);

    /// <summary>Called by the encoding worker. Downloads, encodes all resolutions, uploads results.</summary>
    Task ProcessAsync(VideoId id, CancellationToken ct);
}

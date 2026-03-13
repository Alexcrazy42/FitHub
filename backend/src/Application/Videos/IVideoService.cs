using FitHub.Application.Common;
using FitHub.Application.Files;
using FitHub.Domain.Videos;

namespace FitHub.Application.Videos;

public record VideoUploadInitResult(VideoId VideoId, string PresignedPutUrl);

public record VideoResolutionUrl(VideoQuality Quality, int WidthPx, int HeightPx, int BitrateKbps, string Url);

public record MultipartPartUrl(int PartNumber, string Url);

public record VideoMultipartInitResult(VideoId VideoId, IReadOnlyList<MultipartPartUrl> Parts);

public interface IVideoService
{
    Task<VideoMultipartInitResult> InitMultipartUploadAsync(string title, string fileExtension, long fileSizeBytes, CancellationToken ct);
    Task CompleteMultipartUploadAsync(VideoId id, IReadOnlyList<S3MultipartPart> parts, CancellationToken ct);
    Task<Video> GetAsync(VideoId id, CancellationToken ct);
    Task<PagedResult<Video>> GetAllAsync(PagedQuery query, CancellationToken ct);
    Task<IReadOnlyList<VideoResolutionUrl>> GetResolutionUrlsAsync(VideoId id, CancellationToken ct);
    Task DeleteAsync(VideoId id, CancellationToken ct);

    /// <summary>
    /// Called by the encoding worker. Downloads, encodes all resolutions, uploads results.
    /// </summary>
    Task ProcessAsync(VideoId id, CancellationToken ct);
}

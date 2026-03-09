using FitHub.Domain.Videos;

namespace FitHub.Contracts.V1.Videos;

public record InitVideoUploadResponse(string VideoId, string PresignedPutUrl);

public record VideoResponse(
    string Id,
    string Title,
    string Status,
    int? DurationSeconds,
    string? PosterUrl,
    string? FailureReason,
    DateTimeOffset CreatedAt,
    IReadOnlyList<VideoResolutionResponse> Resolutions);

public record VideoResolutionResponse(
    string Quality,
    int QualityLabel,
    int WidthPx,
    int HeightPx,
    int BitrateKbps,
    long FileSizeBytes);

public record VideoResolutionUrlResponse(
    string Quality,
    int QualityLabel,
    int WidthPx,
    int HeightPx,
    int BitrateKbps,
    string Url);

public static class VideoMappings
{
    public static VideoResponse ToResponse(this Video v, string? posterUrl) => new(
        v.Id.ToString(),
        v.Title,
        v.Status.ToString(),
        v.DurationSeconds,
        posterUrl,
        v.FailureReason,
        v.CreatedAt,
        v.Resolutions.Select(r => r.ToResponse()).ToList());

    public static VideoResolutionResponse ToResponse(this VideoResolution r) => new(
        r.Quality.ToString(),
        (int)r.Quality,
        r.WidthPx,
        r.HeightPx,
        r.BitrateKbps,
        r.FileSizeBytes);
}

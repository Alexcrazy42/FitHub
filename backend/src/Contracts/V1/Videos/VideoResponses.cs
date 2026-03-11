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
    int QualityLabel, // TODO: enum
    int WidthPx,
    int HeightPx,
    int BitrateKbps,
    string Url);

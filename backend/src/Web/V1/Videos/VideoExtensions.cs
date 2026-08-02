using FitHub.Application.Videos;
using FitHub.Contracts.V1.Videos;
using FitHub.Domain.Videos;

namespace FitHub.Web.V1.Videos;

public static class VideoExtensions
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

    public static VideoResolutionUrlResponse ToResponse(this VideoResolutionUrl url)
    {
        return new VideoResolutionUrlResponse(
            url.Quality.ToString(), (int)url.Quality, url.WidthPx, url.HeightPx, url.BitrateKbps, url.Url);
    }
}

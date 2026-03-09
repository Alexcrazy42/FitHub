using FitHub.Application.Videos;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Videos;
using FitHub.Domain.Videos;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Videos;

public class VideoController : ControllerBase
{
    private readonly IVideoService _videoService;
    private readonly IVideoStorageService _storage;

    public VideoController(IVideoService videoService, IVideoStorageService storage)
    {
        _videoService = videoService;
        _storage = storage;
    }

    [HttpGet(ApiRoutesV1.Videos)]
    public async Task<ListResponse<VideoResponse>> GetAll(CancellationToken ct)
    {
        var videos = await _videoService.GetAllAsync(ct);
        var responses = new List<VideoResponse>();
        foreach (var v in videos)
        {
            var posterUrl = v.PosterS3Key is not null
                ? await _storage.GetPresignedDownloadUrlAsync(v.PosterS3Key, TimeSpan.FromHours(2), ct)
                : null;
            responses.Add(v.ToResponse(posterUrl));
        }
        return ListResponse<VideoResponse>.Create(responses);
    }

    [HttpGet(ApiRoutesV1.VideoById)]
    public async Task<VideoResponse> GetById([FromRoute] string id, CancellationToken ct)
    {
        var videoId = VideoId.Parse(id);
        var video = await _videoService.GetAsync(videoId, ct);
        var posterUrl = video.PosterS3Key is not null
            ? await _storage.GetPresignedDownloadUrlAsync(video.PosterS3Key, TimeSpan.FromHours(2), ct)
            : null;
        return video.ToResponse(posterUrl);
    }

    [HttpPost(ApiRoutesV1.VideosInitUpload)]
    public async Task<InitVideoUploadResponse> InitUpload([FromBody] InitVideoUploadRequest? request, CancellationToken ct)
    {
        var title = ValidationException.ThrowIfNull(request?.Title, "Название не может быть пустым");
        var ext = ValidationException.ThrowIfNull(request?.FileExtension, "Расширение файла обязательно");
        var result = await _videoService.InitUploadAsync(title, ext, ct);
        return new InitVideoUploadResponse(result.VideoId.ToString(), result.PresignedPutUrl);
    }

    [HttpPost(ApiRoutesV1.VideoConfirmUpload)]
    public async Task<IActionResult> ConfirmUpload([FromRoute] string id, CancellationToken ct)
    {
        var videoId = VideoId.Parse(id);
        await _videoService.ConfirmUploadAsync(videoId, ct);
        return Accepted();
    }

    [HttpGet(ApiRoutesV1.VideoResolutions)]
    public async Task<ListResponse<VideoResolutionUrlResponse>> GetResolutions([FromRoute] string id, CancellationToken ct)
    {
        var videoId = VideoId.Parse(id);
        var urls = await _videoService.GetResolutionUrlsAsync(videoId, ct);
        var responses = urls.Select(u => new VideoResolutionUrlResponse(
            u.Quality.ToString(), (int)u.Quality, u.WidthPx, u.HeightPx, u.BitrateKbps, u.Url)).ToList();
        return ListResponse<VideoResolutionUrlResponse>.Create(responses);
    }

    [HttpDelete(ApiRoutesV1.VideoById)]
    public async Task<IActionResult> Delete([FromRoute] string id, CancellationToken ct)
    {
        var videoId = VideoId.Parse(id);
        await _videoService.DeleteAsync(videoId, ct);
        return NoContent();
    }
}

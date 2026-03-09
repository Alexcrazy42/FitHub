using FitHub.Application.Files;
using FitHub.Application.Videos;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Videos;
using FitHub.Domain.Videos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Videos;

public class VideoController : ControllerBase
{
    private readonly IVideoService videoService;
    private readonly IS3FileService s3;

    public VideoController(IVideoService videoService, IS3FileService s3)
    {
        this.videoService = videoService;
        this.s3 = s3;
    }

    [HttpGet(ApiRoutesV1.Videos)]
    public async Task<ListResponse<VideoResponse>> GetAll(CancellationToken ct)
    {
        var videos = await videoService.GetAllAsync(ct);
        var responses = new List<VideoResponse>();
        foreach (var v in videos)
        {
            var posterUrl = v.PosterS3Key is not null
                ? await s3.GetPresignedDownloadUrlAsync(v.PosterS3Key, TimeSpan.FromHours(2))
                : null;
            responses.Add(v.ToResponse(posterUrl));
        }

        return ListResponse<VideoResponse>.Create(responses);
    }

    [HttpGet(ApiRoutesV1.VideoById)]
    public async Task<VideoResponse> GetById([FromRoute] string id, CancellationToken ct)
    {
        var videoId = VideoId.Parse(id);
        var video = await videoService.GetAsync(videoId, ct);
        var posterUrl = video.PosterS3Key is not null
            ? await s3.GetPresignedDownloadUrlAsync(video.PosterS3Key, TimeSpan.FromHours(2))
            : null;
        return video.ToResponse(posterUrl);
    }

    [HttpPost(ApiRoutesV1.VideosInitUpload)]
    public async Task<InitVideoUploadResponse> InitUpload([FromBody] InitVideoUploadRequest? request, CancellationToken ct)
    {
        var title = ValidationException.ThrowIfNull(request?.Title, "Название не может быть пустым");
        var ext = ValidationException.ThrowIfNull(request?.FileExtension, "Расширение файла обязательно");
        var result = await videoService.InitUploadAsync(title, ext, ct);
        return new InitVideoUploadResponse(result.VideoId.ToString(), result.PresignedPutUrl);
    }

    [HttpPost(ApiRoutesV1.VideoConfirmUpload)]
    public async Task<IActionResult> ConfirmUpload([FromRoute] string id, CancellationToken ct)
    {
        var videoId = VideoId.Parse(id);
        await videoService.ConfirmUploadAsync(videoId, ct);
        return Accepted();
    }

    [HttpPost(ApiRoutesV1.VideoProcess)]
    [AllowAnonymous]
    public async Task<IActionResult> Process([FromRoute] string id, CancellationToken ct)
    {
        var videoId = VideoId.Parse(id);
        await videoService.ProcessAsync(videoId, ct);
        return NoContent();
    }

    [HttpGet(ApiRoutesV1.VideoResolutions)]
    public async Task<ListResponse<VideoResolutionUrlResponse>> GetResolutions([FromRoute] string id, CancellationToken ct)
    {
        var videoId = VideoId.Parse(id);
        var urls = await videoService.GetResolutionUrlsAsync(videoId, ct);
        var responses = urls.Select(u => new VideoResolutionUrlResponse(
            u.Quality.ToString(), (int)u.Quality, u.WidthPx, u.HeightPx, u.BitrateKbps, u.Url)).ToList();
        return ListResponse<VideoResolutionUrlResponse>.Create(responses);
    }

    [HttpDelete(ApiRoutesV1.VideoById)]
    public async Task<IActionResult> Delete([FromRoute] string id, CancellationToken ct)
    {
        var videoId = VideoId.Parse(id);
        await videoService.DeleteAsync(videoId, ct);
        return NoContent();
    }
}

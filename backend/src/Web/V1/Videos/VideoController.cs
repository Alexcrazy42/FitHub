using FitHub.Application.Files;
using FitHub.Application.Videos;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Videos;
using FitHub.Domain.Videos;
using FitHub.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Videos;

public class VideoController : ControllerBase
{
    private readonly IVideoService videoService;

    public VideoController(IVideoService videoService)
    {
        this.videoService = videoService;
    }

    [HttpGet(ApiRoutesV1.Videos)]
    public async Task<ListResponse<VideoResponse>> GetAll([FromQuery] PagedRequest? paged, CancellationToken ct)
    {
        var query = paged.ToQuery();
        var pagedResult = await videoService.GetAllAsync(query, ct);
        var responses = pagedResult.Items.Select(v => v.ToResponse(v.PosterCachedUrl)).ToList();
        return ListResponse<VideoResponse>.Create(responses, pagedResult.TotalItems!.Value, pagedResult.CurrentPage!.Value, pagedResult.PageSize!.Value);
    }

    [HttpGet(ApiRoutesV1.VideoById)]
    public async Task<VideoResponse> GetById([FromRoute] string id, CancellationToken ct)
    {
        var videoId = VideoId.Parse(id);
        var video = await videoService.GetAsync(videoId, ct);
        return video.ToResponse(video.PosterCachedUrl);
    }

    [HttpPost(ApiRoutesV1.VideosInitMultipartUpload)]
    public async Task<InitVideoMultipartUploadResponse> InitMultipartUpload([FromBody] InitVideoMultipartUploadRequest? request, CancellationToken ct)
    {
        var title = ValidationException.ThrowIfNull(request?.Title, "Название не может быть пустым");
        var ext = ValidationException.ThrowIfNull(request.FileExtension, "Расширение файла обязательно");
        var size = ValidationException.ThrowIfNull(request.FileSizeBytes, "Размер файла обязателен");
        var result = await videoService.InitMultipartUploadAsync(title, ext, size, ct);
        var parts = result.Parts.Select(p => new MultipartPartUrlResponse(p.PartNumber, p.Url)).ToList();
        return new InitVideoMultipartUploadResponse(result.VideoId.ToString(), parts);
    }

    [HttpPost(ApiRoutesV1.VideoCompleteMultipart)]
    public async Task<IActionResult> CompleteMultipart([FromRoute] string id, [FromBody] CompleteVideoMultipartUploadRequest? request, CancellationToken ct)
    {
        var videoId = VideoId.Parse(id);
        var parts = ValidationException.ThrowIfNull(request?.Parts, "Parts обязательны");
        var s3Parts = parts.Select(p => new S3MultipartPart(p.PartNumber, p.ETag)).ToList();
        await videoService.CompleteMultipartUploadAsync(videoId, s3Parts, ct);
        return Accepted();
    }

    // TODO: убрать AllowAnonymous, выписать токен + дать нормальный URL
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
        var responses = urls.Select(VideoExtensions.ToResponse).ToList();
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

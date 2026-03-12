using FFMpegCore;
using FFMpegCore.Enums;
using FitHub.Application.Common;
using FitHub.Application.Files;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Files;
using FitHub.Domain.Videos;
using FitHub.Shared.Common;
using Microsoft.Extensions.Logging;

namespace FitHub.Application.Videos;

internal record EncodeProfile(int Width, int Height, int Crf, string Preset, int AudioKbps, int BitrateKbps);

public class VideoService : IVideoService
{
    private static readonly Dictionary<VideoQuality, EncodeProfile> Profiles = new()
    {
        [VideoQuality.Q360P] = new(640, 360, Crf: 28, "faster", AudioKbps: 96, BitrateKbps: 500),
        [VideoQuality.Q720P] = new(1280, 720, Crf: 23, "faster", AudioKbps: 128, BitrateKbps: 2500),
        [VideoQuality.Q1080P] = new(1920, 1080, Crf: 20, "medium", AudioKbps: 192, BitrateKbps: 5000),
    };

    private static readonly TimeSpan UrlTtl = TimeSpan.FromDays(7);
    private static readonly TimeSpan RefreshThreshold = TimeSpan.FromDays(1);
    private const int MultipartPartSize = 10 * 1024 * 1024; // 10 MB

    private readonly IVideoRepository videoRepository;
    private readonly IFileRepository fileRepository;
    private readonly IS3FileService s3FileService;
    private readonly IVideoEncodingQueue videoQueue;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<VideoService> logger;

    public VideoService(
        IVideoRepository videoRepository,
        IFileRepository fileRepository,
        IS3FileService s3FileService,
        IVideoEncodingQueue videoQueue,
        IUnitOfWork unitOfWork,
        ILogger<VideoService> logger)
    {
        this.videoRepository = videoRepository;
        this.fileRepository = fileRepository;
        this.s3FileService = s3FileService;
        this.videoQueue = videoQueue;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<VideoUploadInitResult> InitUploadAsync(string title, string fileExtension, CancellationToken ct)
    {
        var videoId = VideoId.New();
        var ext = fileExtension.TrimStart('.');
        var s3Key = $"videos/{videoId}/original.{ext}";

        var fileId = FileId.New();
        var fileEntity = FileEntity.Create(fileId, $"original.{ext}", s3Key, FileStatus.WaitingUpload);
        var video = Video.Create(videoId, title.Trim(), fileEntity);

        await fileRepository.PendingAddAsync(fileEntity, ct);
        await videoRepository.PendingAddAsync(video, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var result = await s3FileService.GetPresignedUrlAsync(fileId.ToString(), s3Key);
        return new VideoUploadInitResult(video.Id, result.Url);
    }

    private static string GetVideoContentType(string ext) => ext.ToLowerInvariant() switch
    {
        "mp4" => "video/mp4",
        "webm" => "video/webm",
        "mov" => "video/quicktime",
        "avi" => "video/x-msvideo",
        "mkv" => "video/x-matroska",
        _ => "application/octet-stream",
    };

    public async Task<VideoMultipartInitResult> InitMultipartUploadAsync(
        string title, string fileExtension, long fileSizeBytes, CancellationToken ct)
    {
        var videoId = VideoId.New();
        var ext = fileExtension.TrimStart('.');
        var s3Key = $"videos/{videoId}/original.{ext}";

        var fileId = FileId.New();
        var fileEntity = FileEntity.Create(fileId, $"original.{ext}", s3Key, FileStatus.WaitingUpload);
        var video = Video.Create(videoId, title.Trim(), fileEntity);

        var uploadId = await s3FileService.InitiateMultipartUploadAsync(s3Key, GetVideoContentType(ext));
        fileEntity.SetMultipartUploadId(uploadId);

        await fileRepository.PendingAddAsync(fileEntity, ct);
        await videoRepository.PendingAddAsync(video, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var totalParts = (int)Math.Ceiling((double)fileSizeBytes / MultipartPartSize);
        var partUrls = new List<MultipartPartUrl>(totalParts);
        for (var i = 1; i <= totalParts; i++)
        {
            var url = await s3FileService.GetPresignedPartUrlAsync(s3Key, uploadId, i);
            partUrls.Add(new MultipartPartUrl(i, url));
        }

        return new VideoMultipartInitResult(videoId, partUrls);
    }

    public async Task CompleteMultipartUploadAsync(VideoId id, IReadOnlyList<S3MultipartPart> parts, CancellationToken ct)
    {
        var video = await GetAsync(id, ct);

        if (video.Status != VideoStatus.Pending)
        {
            throw new InvalidOperationException($"Video {id} is not in Pending state.");
        }

        var file = await fileRepository.GetFirstOrDefaultAsync(f => f.Id == video.OriginalFileId, ct);
        if (file is null)
        {
            throw new InvalidOperationException($"Original file for video {id} not found.");
        }

        if (file.MultipartUploadId is null)
        {
            throw new InvalidOperationException($"No multipart upload ID for video {id}.");
        }

        await s3FileService.CompleteMultipartUploadAsync(file.S3Key, file.MultipartUploadId, parts);

        file.SetEntity(video.Id.ToString(), EntityType.Video);
        file.SetStatus(FileStatus.Active);

        await videoQueue.EnqueueAsync(id, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<Video> ConfirmUploadAsync(VideoId id, CancellationToken ct)
    {
        var video = await GetAsync(id, ct);

        if (video.Status != VideoStatus.Pending)
        {
            throw new InvalidOperationException($"Video {id} is not in Pending state.");
        }

        // без outbox, тк максимум ым можем себе позволить опубликовать и упасть на подтверждении файла
        await videoQueue.EnqueueAsync(id, ct);

        var file = await fileRepository.GetFirstOrDefaultAsync(f => f.Id == video.OriginalFileId, ct);
        if (file is not null)
        {
            file.SetEntity(video.Id.ToString(), EntityType.Video);
            file.SetStatus(FileStatus.Active);
            await unitOfWork.SaveChangesAsync(ct);
        }

        return video;
    }

    public async Task<Video> GetAsync(VideoId id, CancellationToken ct)
    {
        var video = await videoRepository.GetWithResolutionsAsync(id, ct);
        NotFoundException.ThrowIfNull(video, "Видео не найдено");

        if (await RefreshVideoMetaUrlsAsync(video, ct))
        {
            await unitOfWork.SaveChangesAsync(ct);
        }
        return video;
    }

    public async Task<PagedResult<Video>> GetAllAsync(PagedQuery query, CancellationToken ct)
    {
        var result = await videoRepository.GetPagedWithResolutionsAsync(query, ct);
        var dirty = false;
        foreach (var video in result.Items)
        {
            dirty |= await RefreshVideoMetaUrlsAsync(video, ct);
        }

        if (dirty)
        {
            await unitOfWork.SaveChangesAsync(ct);
        }
        return result;
    }

    public async Task<IReadOnlyList<VideoResolutionUrl>> GetResolutionUrlsAsync(VideoId id, CancellationToken ct)
    {
        var video = await GetAsync(id, ct); // poster + original URL already refreshed here

        if (video.Status is VideoStatus.Pending or VideoStatus.Processing)
        {
            return [new VideoResolutionUrl(VideoQuality.Original, 0, 0, 0, video.OriginalCachedUrl!)];
        }

        if (video.Status != VideoStatus.Ready)
        {
            return [];
        }

        var dirty = false;
        var urls = new List<VideoResolutionUrl>();
        foreach (var res in video.Resolutions.OrderBy(r => r.Quality))
        {
            if (NeedsRefresh(res.UrlExpiresAt))
            {
                var url = await s3FileService.GetPresignedDownloadUrlAsync(res.S3Key, UrlTtl);
                res.SetCachedUrl(url, DateTimeOffset.UtcNow.Add(UrlTtl));
                dirty = true;
            }
            urls.Add(new VideoResolutionUrl(res.Quality, res.WidthPx, res.HeightPx, res.BitrateKbps, res.CachedUrl!));
        }

        if (dirty)
        {
            await unitOfWork.SaveChangesAsync(ct);
        }

        return urls;
    }

    private static bool NeedsRefresh(DateTimeOffset? expiresAt)
        => expiresAt is null || expiresAt.Value - DateTimeOffset.UtcNow < RefreshThreshold;

    private async Task<bool> RefreshVideoMetaUrlsAsync(Video video, CancellationToken ct)
    {
        var dirty = false;
        var expiresAt = DateTimeOffset.UtcNow.Add(UrlTtl);

        if (video.PosterS3Key is not null && NeedsRefresh(video.PosterUrlExpiresAt))
        {
            var url = await s3FileService.GetPresignedDownloadUrlAsync(video.PosterS3Key, UrlTtl);
            video.SetPosterCachedUrl(url, expiresAt);
            dirty = true;
        }

        if (video.Status is VideoStatus.Pending or VideoStatus.Processing && NeedsRefresh(video.OriginalUrlExpiresAt))
        {
            var url = await s3FileService.GetPresignedDownloadUrlAsync(video.OriginalFile!.S3Key, UrlTtl);
            video.SetOriginalCachedUrl(url, expiresAt);
            dirty = true;
        }

        return dirty;
    }

    public async Task DeleteAsync(VideoId id, CancellationToken ct)
    {
        var video = await GetAsync(id, ct);

        foreach (var res in video.Resolutions)
        {
            try
            { await s3FileService.DeleteFileAsync(res.S3Key); }
            catch { /* best-effort */ }
        }

        if (!String.IsNullOrEmpty(video.PosterS3Key))
        {
            try
            { await s3FileService.DeleteFileAsync(video.PosterS3Key); }
            catch { /* best-effort */ }
        }

        var file = await fileRepository.GetFirstOrDefaultAsync(f => f.Id == video.OriginalFileId, ct);

        if (file is not null)
        {
            try
            {
                await s3FileService.DeleteFileAsync(file.S3Key);
            }
            catch
            {
                 /* best-effort */
            }

            fileRepository.PendingRemove(file);
        }

        videoRepository.PendingRemove(video);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task ProcessAsync(VideoId id, CancellationToken ct)
    {
        var video = await videoRepository.GetWithResolutionsAsync(id, ct);

        NotFoundException.ThrowIfNull(video, $"Video with id {id} not found");

        var originalFile = await fileRepository.GetSingleOrDefaultAsync(f => f.Id == video.OriginalFileId, ct);

        if (originalFile is null)
        {
            logger.LogWarning("ProcessAsync: original file for video {VideoId} not found, skipping", id);
            video.MarkFailed("Original file not found");
            await unitOfWork.SaveChangesAsync(ct);
            return;
        }

        if (video.Status is not VideoStatus.Pending)
        {
            logger.LogDebug("Video {Id} not pending, not processing", video.Id);
            return;
        }

        video.MarkProcessing();
        await unitOfWork.SaveChangesAsync(ct);

        var workDir = Path.Combine(Path.GetTempPath(), "fithub_videos", id.ToString());
        Directory.CreateDirectory(workDir);

        try
        {
            // 1. Download original to temp file
            var originalExt = Path.GetExtension(originalFile.S3Key);
            var originalLocal = Path.Combine(workDir, $"original{originalExt}");

            logger.LogInformation("Downloading original for video {VideoId}", id);
            await using (var stream = await s3FileService.DownloadFileAsync(originalFile.S3Key))
            await using (var file = File.Create(originalLocal))
            {
                await stream.CopyToAsync(file, ct);
            }

            // 2. Probe duration + resolution
            var mediaInfo = await FFProbe.AnalyseAsync(originalLocal, cancellationToken: ct);
            var durationSeconds = (int)mediaInfo.Duration.TotalSeconds;
            var originalHeight = mediaInfo.PrimaryVideoStream?.Height ?? 0;

            logger.LogInformation("Video {VideoId}: {Height}p, {Duration}s", id, originalHeight, durationSeconds);

            // 3. Only encode profiles <= original height; fall back to smallest if original is tiny
            var eligible = Profiles
                .Where(kvp => originalHeight == 0 || kvp.Value.Height <= originalHeight)
                .OrderByDescending(kvp => kvp.Value.Height)
                .ToList();

            if (eligible.Count == 0)
            {
                eligible = [Profiles.MinBy(kvp => kvp.Value.Height)];
            }

            // 4. Encode best-match (highest eligible) profile first → mark Ready immediately
            var (firstQuality, firstProfile) = eligible[0];
            var firstResolution = await EncodeAndUploadAsync(id, firstQuality, firstProfile, originalLocal, workDir, ct);
            video.AddResolution(firstResolution);
            await videoRepository.PendingAddResolutionAsync(firstResolution, ct);

            // TODO: poster не сканится, хз почему — TODO #1
            video.MarkReady(durationSeconds, "");
            await unitOfWork.SaveChangesAsync(ct);
            logger.LogInformation("Video {VideoId} is Ready with {Quality}", id, firstQuality);

            // 5. Encode lower resolutions; per-profile errors don't fail the video
            foreach (var (quality, profile) in eligible.Skip(1))
            {
                try
                {
                    var resolution = await EncodeAndUploadAsync(id, quality, profile, originalLocal, workDir, ct);
                    video.AddResolution(resolution);
                    await videoRepository.PendingAddResolutionAsync(resolution, ct);
                    await unitOfWork.SaveChangesAsync(ct);
                    logger.LogInformation("Video {VideoId} added resolution {Quality}", id, quality);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to encode {Quality} for video {VideoId}, skipping", quality, id);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Encoding failed for video {VideoId}", id);
            video.MarkFailed(ex.Message);
            await unitOfWork.SaveChangesAsync(ct);
        }
        finally
        {
            try
            {
                Directory.Delete(workDir, recursive: true);
            }
            catch (Exception ex)
            {
                // на это мы бы навесили alert + можно сделать джобу в ОС
                logger.LogWarning(ex, "Failed to clean up work dir for video {VideoId}", id);
            }
        }
    }

    private async Task<VideoResolution> EncodeAndUploadAsync(
        VideoId videoId, VideoQuality quality, EncodeProfile profile,
        string originalLocal, string workDir, CancellationToken ct)
    {
        var outputLocal = Path.Combine(workDir, $"{(int)quality}p.mp4");
        var s3Key = $"videos/{videoId}/{(int)quality}p.mp4";

        logger.LogInformation("Encoding {Quality} for video {VideoId}", quality, videoId);

        var scaleFilter =
            $"scale={profile.Width}:{profile.Height}:force_original_aspect_ratio=decrease," +
            $"pad={profile.Width}:{profile.Height}:(ow-iw)/2:(oh-ih)/2";

        await FFMpegArguments
            .FromFileInput(originalLocal)
            .OutputToFile(outputLocal, overwrite: true, options => options
                .WithVideoCodec(VideoCodec.LibX264)
                .WithCustomArgument($"-vf \"{scaleFilter}\"")
                .WithCustomArgument($"-crf {profile.Crf} -preset {profile.Preset}")
                .WithAudioCodec(AudioCodec.Aac)
                .WithCustomArgument($"-b:a {profile.AudioKbps}k")
                .WithCustomArgument("-movflags +faststart"))
            .ProcessAsynchronously(throwOnError: true);

        logger.LogInformation("Uploading {Quality} for video {VideoId}", quality, videoId);
        await using var stream = File.OpenRead(outputLocal);
        await s3FileService.UploadFileAsync(s3Key, stream, "video/mp4");

        var fileSize = new FileInfo(outputLocal).Length;
        return VideoResolution.Create(videoId, quality, fileSize, profile.Width, profile.Height, profile.BitrateKbps, s3Key);
    }
}

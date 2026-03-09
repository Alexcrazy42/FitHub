using FFMpegCore;
using FFMpegCore.Enums;
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

    private readonly IVideoRepository repo;
    private readonly IFileRepository fileRepo;
    private readonly IS3FileService s3;
    private readonly IVideoEncodingQueue queue;
    private readonly IUnitOfWork uow;
    private readonly ILogger<VideoService> logger;

    public VideoService(
        IVideoRepository repo,
        IFileRepository fileRepo,
        IS3FileService s3,
        IVideoEncodingQueue queue,
        IUnitOfWork uow,
        ILogger<VideoService> logger)
    {
        this.repo = repo;
        this.fileRepo = fileRepo;
        this.s3 = s3;
        this.queue = queue;
        this.uow = uow;
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

        await fileRepo.PendingAddAsync(fileEntity, ct);
        await repo.PendingAddAsync(video, ct);
        await uow.SaveChangesAsync(ct);

        var result = await s3.GetPresignedUrlAsync(fileId.ToString(), s3Key);
        return new VideoUploadInitResult(video.Id, result.Url);
    }

    public async Task<Video> ConfirmUploadAsync(VideoId id, CancellationToken ct)
    {
        var video = await GetAsync(id, ct);
        if (video.Status != VideoStatus.Pending)
        {
            throw new InvalidOperationException($"Video {id} is not in Pending state.");
        }

        var file = await fileRepo.GetFirstOrDefaultAsync(f => f.Id == video.OriginalFileId, ct);
        if (file is not null)
        {
            file.SetEntity(video.Id.ToString(), EntityType.Video);
            file.SetStatus(FileStatus.Active);
            await uow.SaveChangesAsync(ct);
        }

        await queue.EnqueueAsync(id, ct);
        return video;
    }

    public async Task<Video> GetAsync(VideoId id, CancellationToken ct)
    {
        var video = await repo.GetWithResolutionsAsync(id, ct);
        NotFoundException.ThrowIfNull(video, "Видео не найдено");
        return video;
    }

    public async Task<IReadOnlyList<Video>> GetAllAsync(CancellationToken ct)
        => await repo.GetAllWithResolutionsAsync(ct);

    public async Task<IReadOnlyList<VideoResolutionUrl>> GetResolutionUrlsAsync(VideoId id, CancellationToken ct)
    {
        var video = await GetAsync(id, ct);
        if (video.Status != VideoStatus.Ready)
        {
            return [];
        }

        var expiry = TimeSpan.FromHours(2);
        var urls = new List<VideoResolutionUrl>();
        foreach (var res in video.Resolutions.OrderBy(r => r.Quality))
        {
            var url = await s3.GetPresignedDownloadUrlAsync(res.ThreeKey, expiry);
            urls.Add(new VideoResolutionUrl(res.Quality, res.WidthPx, res.HeightPx, res.BitrateKbps, url));
        }

        return urls;
    }

    public async Task DeleteAsync(VideoId id, CancellationToken ct)
    {
        var video = await GetAsync(id, ct);

        foreach (var res in video.Resolutions)
        {
            try { await s3.DeleteFileAsync(res.ThreeKey); } catch { /* best-effort */ }
        }

        if (!string.IsNullOrEmpty(video.PosterS3Key))
        {
            try { await s3.DeleteFileAsync(video.PosterS3Key); } catch { /* best-effort */ }
        }

        var file = await fileRepo.GetFirstOrDefaultAsync(f => f.Id == video.OriginalFileId, ct);
        if (file is not null)
        {
            try { await s3.DeleteFileAsync(file.S3Key); } catch { /* best-effort */ }
            fileRepo.PendingRemove(file);
        }

        repo.PendingRemove(video);
        await uow.SaveChangesAsync(ct);
    }

    public async Task ProcessAsync(VideoId id, CancellationToken ct)
    {
        var video = await repo.GetWithResolutionsAsync(id, ct);
        if (video is null)
        {
            logger.LogWarning("ProcessAsync: video {VideoId} not found, skipping", id);
            return;
        }

        var originalFile = await fileRepo.GetFirstOrDefaultAsync(f => f.Id == video.OriginalFileId, ct);
        if (originalFile is null)
        {
            logger.LogWarning("ProcessAsync: original file for video {VideoId} not found, skipping", id);
            video.MarkFailed("Original file not found");
            await uow.SaveChangesAsync(ct);
            return;
        }

        video.MarkProcessing();
        await uow.SaveChangesAsync(ct);

        var workDir = Path.Combine(Path.GetTempPath(), "fithub_videos", id.ToString());
        Directory.CreateDirectory(workDir);

        try
        {
            // 1. Download original to temp file
            var originalExt = Path.GetExtension(originalFile.S3Key);
            var originalLocal = Path.Combine(workDir, $"original{originalExt}");

            logger.LogInformation("Downloading original for video {VideoId}", id);
            await using (var stream = await s3.DownloadFileAsync(originalFile.S3Key))
            await using (var file = File.Create(originalLocal))
            {
                await stream.CopyToAsync(file, ct);
            }

            // 2. Probe duration
            var mediaInfo = await FFProbe.AnalyseAsync(originalLocal, cancellationToken: ct);
            var durationSeconds = (int)mediaInfo.Duration.TotalSeconds;

            // 3. Encode each resolution
            foreach (var (quality, profile) in Profiles)
            {
                var outputLocal = Path.Combine(workDir, $"{(int)quality}p.mp4");
                var s3Key = $"videos/{id}/{(int)quality}p.mp4";

                logger.LogInformation("Encoding {Quality} for video {VideoId}", quality, id);

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

                logger.LogInformation("Uploading {Quality} for video {VideoId}", quality, id);
                await using (var stream = File.OpenRead(outputLocal))
                {
                    await s3.UploadFileAsync(s3Key, stream, "video/mp4");
                }

                var fileSize = new FileInfo(outputLocal).Length;
                var resolution = VideoResolution.Create(id, quality, fileSize,
                    profile.Width, profile.Height, profile.BitrateKbps);

                video.AddResolution(resolution);
                await repo.PendingAddResolutionAsync(resolution, ct);
            }

            // 4. Poster snapshot at 1 second (or midpoint for short clips)
            var posterLocal = Path.Combine(workDir, "poster.jpg");
            var posterS3Key = $"videos/{id}/poster.jpg";
            var snapAt = TimeSpan.FromSeconds(Math.Min(1, durationSeconds / 2.0));
            await FFMpeg.SnapshotAsync(originalLocal, posterLocal, captureTime: snapAt);
            await using (var stream = File.OpenRead(posterLocal))
            {
                await s3.UploadFileAsync(posterS3Key, stream, "image/jpeg");
            }

            video.MarkReady(durationSeconds, posterS3Key);
            await uow.SaveChangesAsync(ct);
            logger.LogInformation("Video {VideoId} encoding completed successfully", id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Encoding failed for video {VideoId}", id);
            video.MarkFailed(ex.Message);
            await uow.SaveChangesAsync(ct);
        }
        finally
        {
            try { Directory.Delete(workDir, recursive: true); } catch { /* best-effort */ }
        }
    }
}

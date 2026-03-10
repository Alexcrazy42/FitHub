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
    // TODO: надо брать не все профайлы, а только <= оригинала + надо выпилить для оригинала
    private static readonly Dictionary<VideoQuality, EncodeProfile> Profiles = new()
    {
        [VideoQuality.Q360P] = new(640, 360, Crf: 28, "faster", AudioKbps: 96, BitrateKbps: 500),
        [VideoQuality.Q720P] = new(1280, 720, Crf: 23, "faster", AudioKbps: 128, BitrateKbps: 2500),
        [VideoQuality.Q1080P] = new(1920, 1080, Crf: 20, "medium", AudioKbps: 192, BitrateKbps: 5000),
    };

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

    public async Task<Video> ConfirmUploadAsync(VideoId id, CancellationToken ct)
    {
        var video = await GetAsync(id, ct);

        if (video.Status != VideoStatus.Pending)
        {
            throw new InvalidOperationException($"Video {id} is not in Pending state.");
        }

        // TODO: outbox
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
        return video;
    }

    public async Task<IReadOnlyList<Video>> GetAllAsync(CancellationToken ct)
        => await videoRepository.GetAllWithResolutionsAsync(ct);

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
            var url = await s3FileService.GetPresignedDownloadUrlAsync(res.S3Key, expiry); // TODO: вычислить один раз и все
            urls.Add(new VideoResolutionUrl(res.Quality, res.WidthPx, res.HeightPx, res.BitrateKbps, url));
        }

        return urls;
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
            { await s3FileService.DeleteFileAsync(file.S3Key); }
            catch { /* best-effort */ }
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
                    await s3FileService.UploadFileAsync(s3Key, stream, "video/mp4");
                }

                var fileSize = new FileInfo(outputLocal).Length;
                var resolution = VideoResolution.Create(id, quality, fileSize,
                    profile.Width, profile.Height, profile.BitrateKbps, s3Key);

                video.AddResolution(resolution);
                await videoRepository.PendingAddResolutionAsync(resolution, ct);
            }

            // 4. Poster snapshot at 1 second (or midpoint for short clips)

            // TODO: фикс что постер не сканится, хз почему
            // var posterLocal = Path.Combine(workDir, "poster.jpg");
            // var posterS3Key = $"videos/{id}/poster.jpg";
            // var snapAt = TimeSpan.FromSeconds(Math.Min(1, durationSeconds / 2.0));
            // await FFMpeg.SnapshotAsync(originalLocal, posterLocal, captureTime: snapAt);
            // await using (var stream = File.OpenRead(posterLocal))
            // {
            //     await s3FileService.UploadFileAsync(posterS3Key, stream, "image/jpeg");
            // }

            video.MarkReady(durationSeconds, "");
            await unitOfWork.SaveChangesAsync(ct);
            logger.LogInformation("Video {VideoId} encoding completed successfully", id);
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
            { Directory.Delete(workDir, recursive: true); }
            catch { /* best-effort */ }
        }
    }
}

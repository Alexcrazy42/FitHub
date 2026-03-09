using FFMpegCore;
using FFMpegCore.Enums;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Videos;
using Microsoft.Extensions.Logging;

namespace FitHub.Application.Videos;

internal record EncodeProfile(int Width, int Height, int Crf, string Preset, int AudioKbps, int BitrateKbps);

public class VideoService : IVideoService
{
    private static readonly Dictionary<VideoQuality, EncodeProfile> Profiles = new()
    {
        [VideoQuality.Q360p] = new(640, 360, Crf: 28, "faster", AudioKbps: 96, BitrateKbps: 500),
        [VideoQuality.Q720p] = new(1280, 720, Crf: 23, "faster", AudioKbps: 128, BitrateKbps: 2500),
        [VideoQuality.Q1080p] = new(1920, 1080, Crf: 20, "medium", AudioKbps: 192, BitrateKbps: 5000),
    };

    private readonly IVideoRepository _repo;
    private readonly IVideoStorageService _storage;
    private readonly VideoEncodingChannel _channel;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<VideoService> _logger;

    public VideoService(
        IVideoRepository repo,
        IVideoStorageService storage,
        VideoEncodingChannel channel,
        IUnitOfWork uow,
        ILogger<VideoService> logger)
    {
        _repo = repo;
        _storage = storage;
        _channel = channel;
        _uow = uow;
        _logger = logger;
    }

    public async Task<VideoUploadInitResult> InitUploadAsync(string title, string fileExtension, CancellationToken ct)
    {
        var video = Video.Create(title.Trim());
        var ext = fileExtension.TrimStart('.');
        var s3Key = $"videos/{video.Id}/original.{ext}";
        video.SetOriginalS3Key(s3Key);

        await _repo.PendingAddAsync(video, ct);
        await _uow.SaveChangesAsync(ct);

        var url = await _storage.GetPresignedUploadUrlAsync(s3Key, ct);
        return new VideoUploadInitResult(video.Id, url);
    }

    public async Task<Video> ConfirmUploadAsync(VideoId id, CancellationToken ct)
    {
        var video = await GetAsync(id, ct);
        if (video.Status != VideoStatus.Pending)
            throw new InvalidOperationException($"Video {id} is not in Pending state.");

        await _channel.Writer.WriteAsync(id, ct);
        return video;
    }

    public async Task<Video> GetAsync(VideoId id, CancellationToken ct)
    {
        var video = await _repo.GetWithResolutionsAsync(id, ct);
        NotFoundException.ThrowIfNull(video, "Видео не найдено");
        return video;
    }

    public async Task<IReadOnlyList<Video>> GetAllAsync(CancellationToken ct)
        => await _repo.GetAllWithResolutionsAsync(ct);

    public async Task<IReadOnlyList<VideoResolutionUrl>> GetResolutionUrlsAsync(VideoId id, CancellationToken ct)
    {
        var video = await GetAsync(id, ct);
        if (video.Status != VideoStatus.Ready)
            return [];

        var expiry = TimeSpan.FromHours(2);
        var urls = new List<VideoResolutionUrl>();
        foreach (var res in video.Resolutions.OrderBy(r => r.Quality))
        {
            var url = await _storage.GetPresignedDownloadUrlAsync(res.S3Key, expiry, ct);
            urls.Add(new VideoResolutionUrl(res.Quality, res.WidthPx, res.HeightPx, res.BitrateKbps, url));
        }
        return urls;
    }

    public async Task DeleteAsync(VideoId id, CancellationToken ct)
    {
        var video = await GetAsync(id, ct);

        foreach (var res in video.Resolutions)
            try { await _storage.DeleteAsync(res.S3Key, ct); } catch { /* best-effort */ }

        if (!string.IsNullOrEmpty(video.PosterS3Key))
            try { await _storage.DeleteAsync(video.PosterS3Key, ct); } catch { /* best-effort */ }

        try { await _storage.DeleteAsync(video.OriginalS3Key, ct); } catch { /* best-effort */ }

        _repo.PendingRemove(video);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task ProcessAsync(VideoId id, CancellationToken ct)
    {
        var video = await _repo.GetWithResolutionsAsync(id, ct);
        if (video is null)
        {
            _logger.LogWarning("ProcessAsync: video {VideoId} not found, skipping", id);
            return;
        }

        video.MarkProcessing();
        await _uow.SaveChangesAsync(ct);

        var workDir = Path.Combine(Path.GetTempPath(), "fithub_videos", id.ToString());
        Directory.CreateDirectory(workDir);

        try
        {
            // 1. Download original to temp file
            var originalExt = Path.GetExtension(video.OriginalS3Key);
            var originalLocal = Path.Combine(workDir, $"original{originalExt}");

            _logger.LogInformation("Downloading original for video {VideoId}", id);
            await using (var stream = await _storage.DownloadAsync(video.OriginalS3Key, ct))
            await using (var file = File.Create(originalLocal))
                await stream.CopyToAsync(file, ct);

            // 2. Probe duration
            var mediaInfo = await FFProbe.AnalyseAsync(originalLocal, cancellationToken: ct);
            var durationSeconds = (int)mediaInfo.Duration.TotalSeconds;

            // 3. Encode each resolution
            foreach (var (quality, profile) in Profiles)
            {
                var outputLocal = Path.Combine(workDir, $"{(int)quality}p.mp4");
                var s3Key = $"videos/{id}/{(int)quality}p.mp4";

                _logger.LogInformation("Encoding {Quality} for video {VideoId}", quality, id);

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

                _logger.LogInformation("Uploading {Quality} for video {VideoId}", quality, id);
                await _storage.UploadFileAsync(s3Key, outputLocal, "video/mp4", ct);

                var fileSize = new FileInfo(outputLocal).Length;
                var resolution = VideoResolution.Create(id, quality, s3Key, fileSize,
                    profile.Width, profile.Height, profile.BitrateKbps);

                video.AddResolution(resolution);
                await _repo.PendingAddResolutionAsync(resolution, ct);
            }

            // 4. Poster snapshot at 1 second (or midpoint for short clips)
            var posterLocal = Path.Combine(workDir, "poster.jpg");
            var posterS3Key = $"videos/{id}/poster.jpg";
            var snapAt = TimeSpan.FromSeconds(Math.Min(1, durationSeconds / 2.0));
            await FFMpeg.SnapshotAsync(originalLocal, posterLocal, captureTime: snapAt);
            await _storage.UploadFileAsync(posterS3Key, posterLocal, "image/jpeg", ct);

            video.MarkReady(durationSeconds, posterS3Key);
            await _uow.SaveChangesAsync(ct);
            _logger.LogInformation("Video {VideoId} encoding completed successfully", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Encoding failed for video {VideoId}", id);
            video.MarkFailed(ex.Message);
            await _uow.SaveChangesAsync(ct);
        }
        finally
        {
            try { Directory.Delete(workDir, recursive: true); } catch { /* best-effort */ }
        }
    }
}

using FitHub.Common.Entities;
using FitHub.Domain.Files;

namespace FitHub.Domain.Videos;

public class Video : IEntity<VideoId>
{
    private List<VideoResolution> resolutions = [];
    private FileEntity? originalFile;

    private Video(
        VideoId id,
        string title,
        FileId originalFileId,
        VideoStatus status,
        DateTimeOffset createdAt)
    {
        Id = id;
        Title = title;
        OriginalFileId = originalFileId;
        Status = status;
        CreatedAt = createdAt;
    }

    public VideoId Id { get; }

    public string Title { get; private set; }

    public FileId OriginalFileId { get; private set; }

    public FileEntity? OriginalFile
    {
        get => UnexpectedException.ThrowIfNull(originalFile, "Файл неожиданно оказался null!");
        private set => originalFile = value;
    }

    public VideoStatus Status { get; private set; }
    public int? DurationSeconds { get; private set; }
    public string? PosterS3Key { get; private set; }
    public string? PosterCachedUrl { get; private set; }
    public DateTimeOffset? PosterUrlExpiresAt { get; private set; }
    public string? OriginalCachedUrl { get; private set; }
    public DateTimeOffset? OriginalUrlExpiresAt { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTimeOffset CreatedAt { get; }

    public IReadOnlyList<VideoResolution> Resolutions => resolutions;

    public void SetPosterCachedUrl(string url, DateTimeOffset expiresAt)
    {
        PosterCachedUrl = url;
        PosterUrlExpiresAt = expiresAt;
    }

    public void SetOriginalCachedUrl(string url, DateTimeOffset expiresAt)
    {
        OriginalCachedUrl = url;
        OriginalUrlExpiresAt = expiresAt;
    }

    public void MarkProcessing() => Status = VideoStatus.Processing;

    public void MarkReady(int durationSeconds, string? posterS3Key)
    {
        Status = VideoStatus.Ready;
        DurationSeconds = durationSeconds;
        PosterS3Key = posterS3Key;
    }

    public void MarkFailed(string reason)
    {
        Status = VideoStatus.Failed;
        FailureReason = reason;
    }

    public void AddResolution(VideoResolution resolution)
    {
        resolutions.Add(resolution);
    }

    public static Video Create(VideoId id, string title, FileEntity originalFile)
    {
        return new Video(id, title, originalFile.Id, VideoStatus.Pending, DateTimeOffset.UtcNow);
    }
}

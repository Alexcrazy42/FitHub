using FitHub.Common.Entities;

namespace FitHub.Domain.Videos;

public class Video : IEntity<VideoId>
{
    private readonly List<VideoResolution> _resolutions = [];

    private Video(
        VideoId id,
        string title,
        VideoStatus status,
        DateTimeOffset createdAt)
    {
        Id = id;
        Title = title;
        OriginalS3Key = string.Empty;
        Status = status;
        CreatedAt = createdAt;
    }

    public VideoId Id { get; }
    public string Title { get; private set; }
    public string OriginalS3Key { get; private set; }
    public VideoStatus Status { get; private set; }
    public int? DurationSeconds { get; private set; }
    public string? PosterS3Key { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTimeOffset CreatedAt { get; }

    public IReadOnlyList<VideoResolution> Resolutions => _resolutions.AsReadOnly();

    // Called immediately after Create(), before persistence, to set the S3 key (which requires the ID).
    public void SetOriginalS3Key(string s3Key) => OriginalS3Key = s3Key;

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

    public void AddResolution(VideoResolution resolution) => _resolutions.Add(resolution);

    public static Video Create(string title)
        => new(VideoId.New(), title, VideoStatus.Pending, DateTimeOffset.UtcNow);
}

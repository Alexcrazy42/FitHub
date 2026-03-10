using FitHub.Common.Entities;

namespace FitHub.Domain.Videos;

public class VideoResolution : IEntity<VideoResolutionId>
{
    private Video? video;

    private VideoResolution(
        VideoResolutionId id,
        VideoId videoId,
        VideoQuality quality,
        long fileSizeBytes,
        int widthPx,
        int heightPx,
        int bitrateKbps,
        string s3Key)
    {
        Id = id;
        VideoId = videoId;
        Quality = quality;
        FileSizeBytes = fileSizeBytes;
        WidthPx = widthPx;
        HeightPx = heightPx;
        BitrateKbps = bitrateKbps;
        S3Key = s3Key;
    }

    public VideoResolutionId Id { get; }

    public VideoId VideoId { get; }

    public Video Video
    {
        get => UnexpectedException.ThrowIfNull(video, "Видео неожиданно оказалось null");
        private set => video = value;
    }

    public VideoQuality Quality { get; private set; }
    public string S3Key { get; private set; }
    public long FileSizeBytes { get; private set; }
    public int WidthPx { get; private set; }
    public int HeightPx { get; private set; }
    public int BitrateKbps { get; private set; }

    public static VideoResolution Create(
        VideoId videoId,
        VideoQuality quality,
        long fileSizeBytes,
        int widthPx,
        int heightPx,
        int bitrateKbps,
        string s3Key)
        => new(VideoResolutionId.New(), videoId, quality, fileSizeBytes, widthPx, heightPx, bitrateKbps, s3Key);
}

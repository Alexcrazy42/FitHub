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
        int bitrateKbps)
    {
        Id = id;
        VideoId = videoId;
        Quality = quality;
        ThreeKey = "s3Key";
        FileSizeBytes = fileSizeBytes;
        WidthPx = widthPx;
        HeightPx = heightPx;
        BitrateKbps = bitrateKbps;
    }

    public VideoResolutionId Id { get; }
    public VideoId VideoId { get; }

    public Video Video
    {
        get => UnexpectedException.ThrowIfNull(video, "Видео неожиданно оказалось null");
        private set => video = value;
    }

    public VideoQuality Quality { get; }
    public string ThreeKey { get; }
    public long FileSizeBytes { get; }
    public int WidthPx { get; }
    public int HeightPx { get; }
    public int BitrateKbps { get; }

    public static VideoResolution Create(
        VideoId videoId,
        VideoQuality quality,
        //string s3Key,
        long fileSizeBytes,
        int widthPx,
        int heightPx,
        int bitrateKbps)
        => new(VideoResolutionId.New(), videoId, quality, fileSizeBytes, widthPx, heightPx, bitrateKbps);
}

using FitHub.Common.Entities;

namespace FitHub.Domain.Videos;

public class VideoResolution : IEntity<VideoResolutionId>
{
    private VideoResolution(
        VideoResolutionId id,
        VideoId videoId,
        VideoQuality quality,
        string s3Key,
        long fileSizeBytes,
        int widthPx,
        int heightPx,
        int bitrateKbps)
    {
        Id = id;
        VideoId = videoId;
        Quality = quality;
        S3Key = s3Key;
        FileSizeBytes = fileSizeBytes;
        WidthPx = widthPx;
        HeightPx = heightPx;
        BitrateKbps = bitrateKbps;
    }

    public VideoResolutionId Id { get; }
    public VideoId VideoId { get; }
    public VideoQuality Quality { get; }
    public string S3Key { get; }
    public long FileSizeBytes { get; }
    public int WidthPx { get; }
    public int HeightPx { get; }
    public int BitrateKbps { get; }

    public static VideoResolution Create(
        VideoId videoId,
        VideoQuality quality,
        string s3Key,
        long fileSizeBytes,
        int widthPx,
        int heightPx,
        int bitrateKbps)
        => new(VideoResolutionId.New(), videoId, quality, s3Key, fileSizeBytes, widthPx, heightPx, bitrateKbps);
}

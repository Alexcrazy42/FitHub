using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Videos;

public class VideoId : GuidIdentifier<VideoId>, IIdentifierDescription
{
    public VideoId(Guid value) : base(value) { }
    public static string EntityTypeName => "Видео";
    public static string Prefix => "video";
}

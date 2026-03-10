using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Videos;

public class VideoResolutionId : GuidIdentifier<VideoResolutionId>, IIdentifierDescription
{
    public VideoResolutionId(Guid value) : base(value) { }
    public static string EntityTypeName => "Разрешение видео";
    public static string Prefix => "vres";
}

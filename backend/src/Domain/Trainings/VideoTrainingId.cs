using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Trainings;

public class VideoTrainingId : GuidIdentifier<VideoTrainingId>, IIdentifierDescription
{
    public VideoTrainingId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Видеотренировка";
    public static string Prefix => "video-training";
}

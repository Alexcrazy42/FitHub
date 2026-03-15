using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Trainings;

public class BaseGroupTrainingPhotoId : GuidIdentifier<BaseGroupTrainingPhotoId>, IIdentifierDescription
{
    public BaseGroupTrainingPhotoId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Фотография базовой групповой тренировки";
    public static string Prefix => "base-group-training-photo-id";
}

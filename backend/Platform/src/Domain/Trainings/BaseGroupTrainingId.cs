using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Trainings;

public class BaseGroupTrainingId : GuidIdentifier<BaseGroupTrainingId>, IIdentifierDescription
{
    public BaseGroupTrainingId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Базовая групповая тренировка";
    public static string Prefix => "base-group-training";
}

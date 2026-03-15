using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Trainings;

public class TrainingTypeId : GuidIdentifier<TrainingTypeId>, IIdentifierDescription
{
    public TrainingTypeId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Тип тренировки";
    public static string Prefix => "training-type";
}

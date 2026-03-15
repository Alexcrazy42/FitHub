using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Trainings;

public class GroupTrainingId : GuidIdentifier<GroupTrainingId>, IIdentifierDescription
{
    public GroupTrainingId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Групповая тренировка";
    public static string Prefix => "group-training";
}

using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Trainings;

public class MuscleGroupId : GuidIdentifier<MuscleGroupId>, IIdentifierDescription
{
    public MuscleGroupId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Группы мышц";
    public static string Prefix => "muscle";
}

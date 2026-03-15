using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Equipments;

public class GymId : GuidIdentifier<GymId>, IIdentifierDescription
{
    public GymId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Спортзал";
    public static string Prefix => "gym";
}

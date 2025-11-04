using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Equipments;

public class GymZoneId : GuidIdentifier<GymZoneId>, IIdentifierDescription
{
    public GymZoneId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Зона в зале";
    public static string Prefix => FormatPrefix("fithub", "zone");
}

using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Equipments;

public class GymEquipmentId : GuidIdentifier<GymEquipmentId>, IIdentifierDescription
{
    public GymEquipmentId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Тренажер в спортзале";
    public static string Prefix => FormatPrefix("fithub", "gym-equipment");
}

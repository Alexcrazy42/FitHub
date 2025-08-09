using FitHub.Entities.Identity;

namespace FitHub.Domain.Equipments;

public class EquipmentId : GuidIdentifier<EquipmentId>, IIdentifierDescription
{
    public EquipmentId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Оборудование";
    public static string Prefix => "equipment";
}

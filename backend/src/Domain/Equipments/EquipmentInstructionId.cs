using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Equipments;

public class EquipmentInstructionId : GuidIdentifier<EquipmentInstructionId>, IIdentifierDescription
{
    public EquipmentInstructionId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Инструкция к тренажеру";
    public static string Prefix => "equipment-instruction";
}

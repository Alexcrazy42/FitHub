using FitHub.Shared.Equipments;

namespace FitHub.Contracts.V1.Equipments.Instructions;

public sealed class UpdateEquipmentInstructionRequest
{
    public string? Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? AdditionalDescription { get; set; }

    public EquipmentInstructionType? EquipmentInstructionType { get; set; }

    public List<string> MuscleGroupIds { get; set; } = [];
}

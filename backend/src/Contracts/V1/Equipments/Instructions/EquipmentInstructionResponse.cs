using FitHub.Contracts.V1.Equipments.MuscleGroups;

namespace FitHub.Contracts.V1.Equipments.Instructions;

public sealed class EquipmentInstructionResponse
{
    public string? Id { get; set; }

    public string? EquipmentId { get; set; }

    public EquipmentResponse? Equipment { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? AdditionalDescription { get; set; }

    public List<MuscleGroupResponse> MuscleGroups { get; set; } = [];
}

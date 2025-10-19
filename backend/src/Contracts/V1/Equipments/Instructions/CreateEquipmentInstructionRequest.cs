namespace FitHub.Contracts.V1.Equipments.Instructions;

public sealed class CreateEquipmentInstructionRequest
{
    public string? EquipmentId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? AdditionalDescription { get; set; }

    public EquipmentInstructionTypeDto? EquipmentInstructionType { get; set; }

    public List<string> MuscleGroupIds { get; set; } = [];

    public List<string> FileIds { get; set; } = [];
}

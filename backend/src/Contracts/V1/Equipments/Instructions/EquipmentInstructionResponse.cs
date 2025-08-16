namespace FitHub.Contracts.V1.Equipments.Instructions;

public sealed class EquipmentInstructionResponse
{
    public Guid? Id { get; set; }

    public Guid? EquipmentId { get; set; }

    public string? VideoUrl { get; set; }
}

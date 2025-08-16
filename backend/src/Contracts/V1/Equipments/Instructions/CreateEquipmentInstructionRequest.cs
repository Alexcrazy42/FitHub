namespace FitHub.Contracts.V1.Equipments.Instructions;

public sealed class CreateEquipmentInstructionRequest
{
    public Guid? EquipmentId { get; set; }

    public string? VideoUrl { get; set; }
}

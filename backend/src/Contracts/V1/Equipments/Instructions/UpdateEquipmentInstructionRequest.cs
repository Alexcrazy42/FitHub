namespace FitHub.Contracts.V1.Equipments.Instructions;

public sealed class UpdateEquipmentInstructionRequest
{
    public Guid? Id { get; set; }

    public Guid? EquipmentId { get; set; }

    public string? VideoUrl { get; set; }
}

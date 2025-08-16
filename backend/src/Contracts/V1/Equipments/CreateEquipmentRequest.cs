namespace FitHub.Contracts.V1.Equipments;

public sealed class CreateEquipmentRequest
{
    public string? Name { get; set; }

    public string? ImageUrl { get; set; }

    public Guid? BrandId { get; set; }

    public DateOnly? InstructionAddBefore { get; private set; }
}

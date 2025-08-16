namespace FitHub.Contracts.V1.Equipments;

public class UpdateEquipmentRequest
{
    public Guid? Id { get; set; }

    public string? Name { get; set; }

    public string? ImageUrl { get; set; }

    public Guid? BrandId { get; set; }

    public DateOnly? InstructionAddBefore { get; set; }
}

namespace FitHub.Contracts.V1.Equipments;

public class UpdateEquipmentRequest
{
    public string? Id { get; set; }

    public string? BrandId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? AdditionalDescroption { get; set; }

    public DateOnly? InstructionAddBefore { get; set; }

    public bool? IsActive { get; set; }
}

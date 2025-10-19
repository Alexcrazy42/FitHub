namespace FitHub.Contracts.V1.Equipments;

public sealed class CreateEquipmentRequest
{
    public string? BrandId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? AdditionalDescroption { get; set; }

    public DateOnly? InstructionAddBefore { get; set; }

    public bool? IsActive { get; set; }
}

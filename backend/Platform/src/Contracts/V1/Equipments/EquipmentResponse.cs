using FitHub.Contracts.V1.Equipments.Brands;
using FitHub.Contracts.V1.Equipments.Instructions;

namespace FitHub.Contracts.V1.Equipments;

public sealed class EquipmentResponse
{
    public string? Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? AdditionalDescroption { get; set; }

    public DateOnly? InstructionAddBefore { get; set; }

    public BrandResponse? Brand { get; set; }

    public bool? IsActive { get; set; }

    public IReadOnlyList<EquipmentInstructionResponse> Instructions { get; set; } = [];
}

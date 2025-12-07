using FitHub.Common.Entities;
using FitHub.Domain.Trainings;

namespace FitHub.Domain.Equipments;

public class Equipment : IEntity<EquipmentId>
{
    private readonly List<EquipmentInstruction> instructions = [];
    private Brand? brand;

    private Equipment(EquipmentId id, string name, BrandId brandId, bool isActive)
    {
        Id = id;
        Name = name;
        BrandId = brandId;
        IsActive = isActive;
    }

    public EquipmentId Id { get; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public string? AdditionalDescroption { get; private set; }

    public DateOnly? InstructionAddBefore { get; private set; }

    public bool IsActive { get; private set; }

    public BrandId BrandId { get; private set; }

    public Brand Brand
    {
        get => UnexpectedException.ThrowIfNull(brand, "Брэнд неожиданно оказался null");
        private set => brand = value;
    }

    public IReadOnlyList<EquipmentInstruction> Instructions => instructions;

    public void SetName(string name) => Name = name;
    public void SetDescription(string description) => Description = description;
    public void SetAdditionalDescription(string description) => AdditionalDescroption = description;
    public void SetInstructionAddBefore(DateOnly date) => InstructionAddBefore = date;
    public void SetActive(bool isActive) => IsActive = isActive;

    public void SetBrand(Brand newBrand)
    {
        Brand = newBrand;
        BrandId = newBrand.Id;
    }

    public static Equipment Create(string name, BrandId brandId, bool isActive,
        string? description = null,
        string? additionalDescroption = null,
        DateOnly? instructionAddBefore = null)
    {
        return new Equipment(EquipmentId.New(), name, brandId, isActive)
        {
            Description = description,
            AdditionalDescroption = additionalDescroption,
            InstructionAddBefore = instructionAddBefore
        };
    }
}

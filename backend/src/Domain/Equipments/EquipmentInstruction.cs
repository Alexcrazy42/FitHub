using FitHub.Common.Entities;
using FitHub.Domain.Trainings;

namespace FitHub.Domain.Equipments;

public class EquipmentInstruction : IEntity<EquipmentInstructionId>
{
    private Equipment? equipment;
    private List<MuscleGroup> muscleGroups = [];

    public EquipmentInstruction(EquipmentInstructionId id, string name)
    {
        Id = id;
        Name = name;
    }

    public EquipmentInstructionId Id { get; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public string? AdditionalDescription { get; private set; }

    public Equipment Equipment
    {
        get => UnexpectedException.ThrowIfNull(equipment, "Тренажер неожиданно стал null");
        set => equipment = value;
    }

    public List<MuscleGroup> MuscleGroups => muscleGroups;
}

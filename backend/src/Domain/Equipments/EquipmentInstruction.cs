using FitHub.Common.Entities;
using FitHub.Domain.Trainings;

namespace FitHub.Domain.Equipments;

public class EquipmentInstruction : IEntity<EquipmentInstructionId>
{
    private Equipment? equipment;
    private List<MuscleGroup> muscleGroups = [];

    private EquipmentInstruction(EquipmentInstructionId id, string name, EquipmentId equipmentId, EquipmentInstructionType intructionType)
    {
        Id = id;
        Name = name;
        EquipmentId = equipmentId;
        IntructionType = intructionType;
    }

    public EquipmentInstructionId Id { get; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public string? AdditionalDescription { get; private set; }

    public EquipmentInstructionType IntructionType { get; private set; }

    public EquipmentId EquipmentId { get; private set; }

    public Equipment Equipment
    {
        get => UnexpectedException.ThrowIfNull(equipment, "Тренажер неожиданно стал null");
        set => equipment = value;
    }

    public List<MuscleGroup> MuscleGroups => muscleGroups;

    public static EquipmentInstruction Create(string name,
        Equipment equipment,
        EquipmentInstructionType instructionType,
        List<MuscleGroup> muscleGroups,
        string? description = null,
        string? additionalDescription = null)
    {
        var entity = new EquipmentInstruction(EquipmentInstructionId.New(), name, equipment.Id, instructionType)
        {
            Equipment = equipment,
            Description = description,
            AdditionalDescription = additionalDescription
        };
        entity.MuscleGroups.AddRange(muscleGroups);
        return entity;
    }
}

using FitHub.Common.Entities;
using FitHub.Domain.Trainings;

namespace FitHub.Domain.Equipments;

public class Equipment : IEntity<EquipmentId>
{
    private readonly List<EquipmentInstruction> instructions = [];
    private readonly List<Gym> gyms = [];

    public Equipment(EquipmentId id, string name)
    {
        Id = id;
        Name = name;
    }

    public EquipmentId Id { get; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public string? AdditionalDescroption { get; private set; }

    public DateOnly? InstructionAddBefore { get; private set; }

    public IReadOnlyList<EquipmentInstruction> Instructions => instructions;

    public IReadOnlyList<Gym> Gyms => gyms;
}

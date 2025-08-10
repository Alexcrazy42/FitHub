using FitHub.Common.Entities;

namespace FitHub.Domain.Equipments;

public class Gym : IEntity<GymId>
{
    private List<Equipment> equipments = [];

    public Gym(GymId id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public GymId Id { get; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public IReadOnlyList<Equipment> Equipments => equipments;

    public void AddEquipments(IReadOnlyList<Equipment> newEquipments)
    {
        equipments.AddRange(newEquipments);
    }
}

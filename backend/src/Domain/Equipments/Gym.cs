using FitHub.Common.Entities;
using FitHub.Domain.Users;

namespace FitHub.Domain.Equipments;

public class Gym : IEntity<GymId>
{
    private List<Equipment> equipments = [];
    private List<GymAdmin> admins = [];
    private List<GymZone> zones = [];

    private Gym(GymId id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public GymId Id { get; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public IReadOnlyList<Equipment> Equipments => equipments;

    public IReadOnlyList<GymAdmin> Admins => admins;

    public IReadOnlyList<GymZone> Zones => zones;

    public void AddEquipments(IReadOnlyList<Equipment> newEquipments)
    {
        equipments.AddRange(newEquipments);
    }

    public void UpdateName(string name) => Name = name;

    public void UpdateDescription(string description) => Description = description;

    public static Gym Create(string name, string description)
    {
        return new Gym(GymId.New(), name, description);
    }
}

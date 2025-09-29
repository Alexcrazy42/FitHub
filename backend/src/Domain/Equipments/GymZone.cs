using FitHub.Common.Entities;

namespace FitHub.Domain.Equipments;

public class GymZone : IEntity<GymZoneId>
{
    public GymZone(GymZoneId id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public GymZoneId Id { get; }

    public string Name { get; private set; }

    public string Description { get; private set; }
}

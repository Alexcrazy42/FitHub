using FitHub.Common.Entities;

namespace FitHub.Domain.Equipments;

public class GymZone : IEntity<GymZoneId>
{
    private List<Gym> gyms = [];

    private GymZone(GymZoneId id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public GymZoneId Id { get; }

    public string Name { get; set; }

    public string Description { get; set; }

    public List<Gym> Gyms => gyms;

    public static GymZone Create(string name, string description)
    {
        return new GymZone(GymZoneId.New(), name, description);
    }
}

using FitHub.Common.Entities;
using FitHub.Domain.Files;
using FitHub.Domain.Users;

namespace FitHub.Domain.Equipments;

public class Gym : IEntity<GymId>
{
    private List<GymAdmin> admins = [];
    private List<Trainer> trainers = [];
    private List<GymZone> zones = [];
    private List<VisitorGymRelation> visitors = [];
    private List<GymEquipment> gymEquipments = [];

    private Gym(GymId id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public GymId Id { get; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public IReadOnlyList<GymEquipment> GymEquipments => gymEquipments;

    public IReadOnlyList<GymAdmin> Admins => admins;

    public IReadOnlyList<Trainer> Trainers => trainers;

    public IReadOnlyList<GymZone> Zones => zones;

    public IReadOnlyList<VisitorGymRelation> Visitors => visitors;

    public IReadOnlyList<FileEntity> Files { get; private set; } = [];

    public void SetFiles(IReadOnlyList<FileEntity> files) => Files = files;

    public void UpdateName(string name) => Name = name;

    public void UpdateDescription(string description) => Description = description;

    public static Gym Create(string name, string description)
    {
        return new Gym(GymId.New(), name, description);
    }
}

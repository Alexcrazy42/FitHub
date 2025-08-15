using FitHub.Common.Entities;
using FitHub.Domain.Equipments;

namespace FitHub.Domain.Users;

public class GymAdmin : IEntity<GymAdminId>
{
    private readonly List<Gym> gyms = [];

    public GymAdmin(GymAdminId id)
    {
        Id = id;
    }

    public GymAdminId Id { get; }

    public IReadOnlyCollection<Gym> Gyms => gyms;
}

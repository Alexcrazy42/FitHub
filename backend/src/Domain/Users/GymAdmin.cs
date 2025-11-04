using FitHub.Common.Entities;
using FitHub.Domain.Equipments;

namespace FitHub.Domain.Users;

public class GymAdmin : IEntity<GymAdminId>
{
    private List<Gym> gyms  = [];

    private GymAdmin(GymAdminId id)
    {
        Id = id;
    }

    public GymAdminId Id { get; }

    public IReadOnlyList<Gym> Gyms => gyms;

    public void SetGym(Gym gym)
    {
        gyms = [gym];
    }

    public static GymAdmin Create()
    {
        var gymAdmin = new GymAdmin(GymAdminId.New());
        return gymAdmin;
    }
}

using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities;
using FitHub.Domain.Equipments;

namespace FitHub.Domain.Users;

public class GymAdmin : IEntity<GymAdminId>
{
    private List<Gym> gyms  = [];
    private User? user;

    private GymAdmin(GymAdminId id, IdentityUserId userId)
    {
        Id = id;
        UserId = userId;
    }

    public GymAdminId Id { get; }

    public IdentityUserId UserId { get; private set; }

    public User User
    {
        get => UnexpectedException.ThrowIfNull(user, "Пользователь неожиданно оказался null");
        private set => user = value;
    }

    public IReadOnlyList<Gym> Gyms => gyms;

    public void SetGym(Gym gym)
    {
        gyms = [gym];
    }

    public static GymAdmin Create(User user)
    {
        var gymAdmin = new GymAdmin(GymAdminId.New(), user.Id);
        return gymAdmin;
    }
}

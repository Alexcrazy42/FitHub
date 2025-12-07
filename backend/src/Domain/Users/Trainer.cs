using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities;
using FitHub.Domain.Equipments;

namespace FitHub.Domain.Users;

public class Trainer : IEntity<TrainerId>
{
    private User? user;
    private List<Gym> gyms = [];

    public Trainer(TrainerId id, IdentityUserId userId)
    {
        Id = id;
        UserId = userId;
    }

    public TrainerId Id { get; }

    public IdentityUserId UserId { get; private set; }

    public IReadOnlyList<Gym> Gyms => gyms;

    public User User
    {
        get => UnexpectedException.ThrowIfNull(user, "Пользоваль неожиданно оказался null");
        private set => user = value;
    }

    public void SetGym(Gym gym)
    {
        gyms = [gym];
    }

    public static Trainer Create(User user)
    {
        return new Trainer(TrainerId.New(), user.Id);
    }
}

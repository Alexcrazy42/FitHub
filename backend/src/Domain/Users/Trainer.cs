using FitHub.Common.Entities;

namespace FitHub.Domain.Users;

public class Trainer : IEntity<TrainerId>
{
    private User? user;

    public Trainer(TrainerId id, UserId userId)
    {
        Id = id;
        UserId = userId;
    }

    public TrainerId Id { get; }

    public UserId UserId { get; private set; }

    public User User
    {
        get => UnexpectedException.ThrowIfNull(user, "Пользоваль неожиданно оказался null");
        private set => user = value;
    }
}

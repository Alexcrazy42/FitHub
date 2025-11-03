using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities;

namespace FitHub.Domain.Users;

public class Trainer : IEntity<TrainerId>
{
    private User? user;

    public Trainer(TrainerId id, IdentityUserId userId)
    {
        Id = id;
        UserId = userId;
    }

    public TrainerId Id { get; }

    public IdentityUserId UserId { get; private set; }

    public User User
    {
        get => UnexpectedException.ThrowIfNull(user, "Пользоваль неожиданно оказался null");
        private set => user = value;
    }

    public static Trainer Create(User user)
    {
        return new Trainer(TrainerId.New(), user.Id);
    }
}

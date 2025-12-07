using FitHub.Common.Entities;
using FitHub.Domain.Users;

namespace FitHub.Domain.Equipments;

public class VisitorGymRelation
{
    private Gym? gym;
    private Visitor? visitor;

    private VisitorGymRelation(GymId gymId, VisitorId visitorId, bool isDefaultGym, int visitCount)
    {
        GymId = gymId;
        VisitorId = visitorId;
        IsDefaultGym = isDefaultGym;
        VisitCount = visitCount;
    }

    public GymId GymId { get; init; }

    public Gym Gym
    {
        get => UnexpectedException.ThrowIfNull(gym, "Зал неожиданно оказался null");
        private set => gym = value;
    }

    public VisitorId VisitorId { get; init; }

    public Visitor Visitor
    {
        get => UnexpectedException.ThrowIfNull(visitor, "Посетитель неожиданно оказался null");
        private set => visitor = value;
    }

    public bool IsDefaultGym { get; private set; }

    public int VisitCount { get; private set; }

    public static VisitorGymRelation Create(Gym gym, Visitor visitor, bool isDefaultGym)
    {
        return new VisitorGymRelation(gym.Id, visitor.Id, isDefaultGym, 0);
    }
}

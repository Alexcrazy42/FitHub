using FitHub.Common.Entities;
using FitHub.Domain.Users;

namespace FitHub.Domain.Trainings;

public class PersonalTraining : IEntity<PersonalTrainingId>, IBaseTrainingWithSchedule
{
    private Visitor? visitor;
    private Trainer? trainer;

    public PersonalTraining(PersonalTrainingId id, DateTimeOffset startTime, DateTimeOffset endTime, VisitorId visitorId, TrainerId trainerId)
    {
        Id = id;
        StartTime = startTime;
        EndTime = endTime;
        VisitorId = visitorId;
        TrainerId = trainerId;
    }

    public PersonalTrainingId Id { get; }

    public DateTimeOffset StartTime { get; private set; }

    public DateTimeOffset EndTime { get; private set; }

    public VisitorId VisitorId { get; private set; }

    public Visitor Visitor
    {
        get => UnexpectedException.ThrowIfNull(visitor, "Пользователь неожиданно стал null");
        private set => visitor = value;
    }

    public TrainerId TrainerId { get; private set; }

    public Trainer Trainer
    {
        get => UnexpectedException.ThrowIfNull(trainer, "Тренер неожиданно оказался null");
        private set => trainer = value;
    }

    public void SetSchedule(DateTimeOffset startTime, DateTimeOffset endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }
}

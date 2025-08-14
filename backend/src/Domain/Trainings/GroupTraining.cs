using FitHub.Common.Entities;
using FitHub.Domain.Users;

namespace FitHub.Domain.Trainings;

public class GroupTraining : IEntity<GroupTrainingId>, IBaseTrainingWithSchedule
{
    private BaseGroupTraining? baseGroupTraining;
    private Trainer? trainer;
    private List<Visitor> participants = [];

    public GroupTraining(GroupTrainingId id, BaseGroupTrainingId baseGroupTrainingId, TrainerId trainerId)
    {
        Id = id;
        TrainerId = trainerId;
        BaseGroupTrainingId = baseGroupTrainingId;
    }

    public GroupTrainingId Id { get; }

    public BaseGroupTrainingId BaseGroupTrainingId { get; private set; }

    public BaseGroupTraining BaseGroupTraining
    {
        get => UnexpectedException.ThrowIfNull(baseGroupTraining, "Базовая тренировка неожиданно оказалась null");
        private set => baseGroupTraining = value;
    }

    public TrainerId TrainerId { get; private set; }

    public Trainer Trainer
    {
        get => UnexpectedException.ThrowIfNull(trainer, "Тренер неожиданно оказался null");
        private set => trainer = value;
    }

    public IReadOnlyList<Visitor> Participants => participants;

    public DateTimeOffset StartTime { get; private set; }

    public DateTimeOffset EndTime { get; private set; }

    public void SetSchedule(DateTimeOffset startTime, DateTimeOffset endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }

    public void AddParticipant(Visitor participant)
    {
        participants.Add(participant);
    }
}

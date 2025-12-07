using FitHub.Common.Entities;
using FitHub.Domain.Equipments;
using FitHub.Domain.Users;

namespace FitHub.Domain.Trainings;

public class GroupTraining : IEntity<GroupTrainingId>, IBaseTrainingWithSchedule
{
    private BaseGroupTraining? baseGroupTraining;
    private Gym? gym;
    private Trainer? trainer;
    private List<Visitor> participants = [];

    private GroupTraining(GroupTrainingId id, BaseGroupTrainingId baseGroupTrainingId, GymId gymId, TrainerId trainerId, DateTimeOffset startTime, DateTimeOffset endTime)
    {
        Id = id;
        GymId = gymId;
        TrainerId = trainerId;
        BaseGroupTrainingId = baseGroupTrainingId;
        StartTime = startTime;
        EndTime = endTime;
    }

    public GroupTrainingId Id { get; }

    public BaseGroupTrainingId BaseGroupTrainingId { get; private set; }

    public BaseGroupTraining BaseGroupTraining
    {
        get => UnexpectedException.ThrowIfNull(baseGroupTraining, "Базовая тренировка неожиданно оказалась null");
        private set => baseGroupTraining = value;
    }

    public GymId? GymId { get; private set; }

    public Gym Gym
    {
        get => UnexpectedException.ThrowIfNull(gym, "Зал неожиданно оказался null");
        private set => gym = value;
    }

    public TrainerId TrainerId { get; private set; }

    public Trainer Trainer
    {
        get => UnexpectedException.ThrowIfNull(trainer, "Тренер неожиданно оказался null!");
        private set => trainer = value;
    }

    public IReadOnlyList<Visitor> Participants => participants;

    public DateTimeOffset StartTime { get; private set; }

    public DateTimeOffset EndTime { get; private set; }

    public bool IsActive { get; private set; }

    public void SetTrainer(Trainer trainer)
    {
        TrainerId = trainer.Id;
        Trainer = trainer;
    }

    public void SetBaseGroupTraining(BaseGroupTraining baseGroupTraining)
    {
        BaseGroupTrainingId = baseGroupTraining.Id;
        BaseGroupTraining = baseGroupTraining;
    }

    public void SetSchedule(DateTimeOffset startTime, DateTimeOffset endTime)
    {
        if (startTime >= endTime)
        {
            throw new ValidationException("Время начала должно быть раньше время конца!");
        }

        StartTime = startTime;
        EndTime = endTime;
    }

    public void AddParticipant(Visitor participant)
    {
        participants.Add(participant);
    }

    public void SetActive(bool isActive)
    {
        if (TrainerId is null && isActive)
        {
            throw new ValidationException("Невозможно сделать тренировку активной без тренера!");
        }
        IsActive = isActive;
    }

    public void SetParticipants(List<Visitor> newParticipants)
    {
        participants = newParticipants;
    }

    public static GroupTraining Create(BaseGroupTraining baseGroupTraining, Gym gym, Trainer trainer, DateTimeOffset startTime, DateTimeOffset endTime)
    {
        return new GroupTraining(GroupTrainingId.New(), baseGroupTraining.Id, gym.Id, trainer.Id, startTime, endTime);
    }
}

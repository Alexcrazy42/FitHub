namespace FitHub.Domain.Trainings;

public interface IBaseTrainingWithSchedule
{
    public DateTimeOffset StartTime { get;  }

    public DateTimeOffset EndTime { get; }

    public void SetSchedule(DateTimeOffset startTime, DateTimeOffset endTime);
}

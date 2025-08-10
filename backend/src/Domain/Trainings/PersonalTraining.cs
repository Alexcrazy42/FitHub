using FitHub.Common.Entities;

namespace FitHub.Domain.Trainings;

public class PersonalTraining : IEntity<PersonalTrainingId>
{
    public PersonalTraining(PersonalTrainingId id)
    {
        Id = id;
    }

    public PersonalTrainingId Id { get; }
}

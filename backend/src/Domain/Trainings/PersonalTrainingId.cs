using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Trainings;

public class PersonalTrainingId : GuidIdentifier<PersonalTrainingId>, IIdentifierDescription
{
    public PersonalTrainingId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Тренировка";
    public static string Prefix => "training";
}

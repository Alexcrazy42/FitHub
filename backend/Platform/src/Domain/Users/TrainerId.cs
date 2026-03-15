using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Users;

public class TrainerId : GuidIdentifier<TrainerId>, IIdentifierDescription
{
    public TrainerId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Тренер";
    public static string Prefix => "trainer-id";
}

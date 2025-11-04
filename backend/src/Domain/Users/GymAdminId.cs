using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Users;

public class GymAdminId : GuidIdentifier<GymAdminId>, IIdentifierDescription
{
    public GymAdminId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Администратор зала";
    public static string Prefix => "gym-admin";
}

using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Users;

public class UserId(Guid value) : GuidIdentifier<UserId>(value), IIdentifierDescription
{
    public static string EntityTypeName => "Пользователь";
    public static string Prefix => "user";
}

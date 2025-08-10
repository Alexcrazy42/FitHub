using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Users;

public class UserRefreshTokenId : GuidIdentifier<UserRefreshTokenId>, IIdentifierDescription
{
    public UserRefreshTokenId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Рефреш токен";
    public static string Prefix => "user-refresh-token-id";
}

using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Users;

public class TokenId : GuidIdentifier<TokenId>, IIdentifierDescription
{
    public TokenId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Токен";
    public static string Prefix => "token";
}

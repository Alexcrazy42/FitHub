using FitHub.Common.Entities.Identity;

namespace FitHub.Authentication;

public sealed class IdentityUserId : GuidIdentifier<IdentityUserId>, IIdentifierDescription
{
    public IdentityUserId(Guid value) : base(value)
    {
    }



    public static string EntityTypeName => "Идентификатор пользователя";
    public static string Prefix => String.Empty;
}

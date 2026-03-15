using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Users;

public class SessionId : GuidIdentifier<SessionId>, IIdentifierDescription
{
    public SessionId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Сессия пользователя";
    public static string Prefix => "session";
}

using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Users;

public class VisitorId : GuidIdentifier<VisitorId>, IIdentifierDescription
{
    public VisitorId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Посетитель зала";
    public static string Prefix => "visitor";
}

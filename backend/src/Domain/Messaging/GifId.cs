using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Messaging;

public class GifId : GuidIdentifier<GifId>, IIdentifierDescription
{
    public GifId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Гиф";
    public static string Prefix => FormatPrefix("fithub", "gif");
}

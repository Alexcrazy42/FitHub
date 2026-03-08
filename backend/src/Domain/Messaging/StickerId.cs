using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Messaging;

public class StickerId : GuidIdentifier<StickerId>, IIdentifierDescription
{
    public StickerId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Стикер";
    public static string Prefix => FormatPrefix("fithub", "sticker");
}

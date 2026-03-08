using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Messaging;

public sealed class StickerGroupId : GuidIdentifier<StickerGroupId>, IIdentifierDescription
{
    public StickerGroupId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Группа стикеров";
    public static string Prefix => FormatPrefix("fithub", "sticker-group");
}

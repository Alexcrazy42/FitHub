using FitHub.Contracts.V1.Messaging.Stickers;
using FitHub.Domain.Messaging;

namespace FitHub.Web.V1.Messaging;

public static class StickerResponseExtensions
{
    public static StickerGroupResponse ToResponse(this StickerGroup group)
    {
        return new StickerGroupResponse
        {
            Id = group.Id.ToString(),
            Name = group.Name,
            IsActive = group.IsActive
        };
    }

    public static StickerResponse ToResponse(this Sticker sticker)
    {
        return new StickerResponse
        {
            Id = sticker.Id.ToString(),
            Name = sticker.Name,
            GroupId = sticker.GroupId.ToString(),
            FileId = sticker.FileId.ToString(),
            Position = sticker.Position
        };
    }
}

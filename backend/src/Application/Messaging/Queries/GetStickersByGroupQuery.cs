using FitHub.Domain.Messaging;

namespace FitHub.Application.Messaging.Queries;

public class GetStickersByGroupQuery
{
    public StickerGroupId GroupId { get; init; }

    public GetStickersByGroupQuery(StickerGroupId groupId)
    {
        GroupId = groupId;
    }
}

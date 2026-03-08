using FitHub.Domain.Files;
using FitHub.Domain.Messaging;

namespace FitHub.Application.Messaging.Commands;

public class AddStickerCommand
{
    public StickerGroupId GroupId { get; init; }

    public FileId FileId { get; init; }

    public string Name { get; init; }

    public AddStickerCommand(StickerGroupId groupId, FileId fileId, string name)
    {
        GroupId = groupId;
        FileId = fileId;
        Name = name;
    }
}

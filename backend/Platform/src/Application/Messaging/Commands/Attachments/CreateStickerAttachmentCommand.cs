using FitHub.Domain.Files;
using FitHub.Domain.Messaging;

namespace FitHub.Application.Messaging.Commands.Attachments;

public class CreateStickerAttachmentCommand
{
    public StickerId StickerId { get; init; }

    public FileId FileId { get; init; }

    public string Name { get; init; }

    public CreateStickerAttachmentCommand(StickerId stickerId, FileId fileId, string name)
    {
        StickerId = stickerId;
        FileId = fileId;
        Name = name;
    }
}

using FitHub.Domain.Files;

namespace FitHub.Domain.Messaging.Attachments;

public class StickerAttachment
{
    public StickerId StickerId { get; set; }

    public FileId FileId { get; set; }

    public string Name { get; set; }

    public StickerAttachment(StickerId stickerId, FileId fileId, string name)
    {
        StickerId = stickerId;
        FileId = fileId;
        Name = name;
    }
}

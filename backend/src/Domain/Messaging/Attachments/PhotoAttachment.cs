using FitHub.Domain.Files;

namespace FitHub.Domain.Messaging.Attachments;

public class PhotoAttachment
{
    public FileId FileId { get; set; }

    public PhotoAttachment(FileId fileId)
    {
        FileId = fileId;
    }
}

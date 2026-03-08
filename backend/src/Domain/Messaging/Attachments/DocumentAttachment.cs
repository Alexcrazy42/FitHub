using FitHub.Domain.Files;

namespace FitHub.Domain.Messaging.Attachments;

public class DocumentAttachment
{
    public FileId FileId { get; set; }
    public string FileName { get; set; }
    public long FileSize { get; set; }
    public string MimeType { get; set; }

    public DocumentAttachment(FileId fileId, string fileName, long fileSize, string mimeType)
    {
        FileId = fileId;
        FileName = fileName;
        FileSize = fileSize;
        MimeType = mimeType;
    }
}

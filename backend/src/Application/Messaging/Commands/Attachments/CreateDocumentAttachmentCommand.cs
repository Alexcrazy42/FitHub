using FitHub.Domain.Files;

namespace FitHub.Application.Messaging.Commands.Attachments;

public class CreateDocumentAttachmentCommand
{
    public FileId FileId { get; init; }
    public string FileName { get; init; }
    public long FileSize { get; init; }
    public string MimeType { get; init; }

    public CreateDocumentAttachmentCommand(FileId fileId, string fileName, long fileSize, string mimeType)
    {
        FileId = fileId;
        FileName = fileName;
        FileSize = fileSize;
        MimeType = mimeType;
    }
}

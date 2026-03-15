namespace FitHub.Contracts.V1.Messaging.Messages.Attachments;

public class CreateDocumentAttachmentRequest
{
    public string? FileId { get; set; }
    public string? FileName { get; set; }
    public long FileSize { get; set; }
    public string? MimeType { get; set; }
}

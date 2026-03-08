namespace FitHub.Contracts.V1.Messaging.Messages.Attachments;

public class CreateStickerAttachmentRequest
{
    public string? StickerId { get; set; }

    public string? FileId { get; set; }

    public string? Name { get; set; }
}

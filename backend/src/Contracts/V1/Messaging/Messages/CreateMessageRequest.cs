using FitHub.Contracts.V1.Messaging.Messages.Attachments;

namespace FitHub.Contracts.V1.Messaging.Messages;

public class CreateMessageRequest
{
    public string? ChatId { get; set; }

    public string? MessageText { get; set; }

    public string? ReplyMessageId { get; set; }

    public List<CreateLinkAttachmentRequest> Links { get; set; } = [];

    public List<CreateTagUserAttachmentRequest> Tags { get; set; } = [];

    public List<CreatePhotoAttachmentRequest> Photos { get; set; } = [];

    public List<CreateStickerAttachmentRequest> Stickers { get; set; } = [];
}

using FitHub.Contracts.V1.Messaging.Messages.Attachments;

namespace FitHub.Contracts.V1.Messaging.Messages;

public class UpdateMessageRequest
{
    public string? MessageText { get; set; }

    public string? ReplyMessageId { get; set; }

    public List<CreateLinkAttachmentRequest> Links { get; set; } = [];

    public List<CreateTagUserAttachmentRequest> Tags { get; set; } = [];

    public List<CreatePhotoAttachmentRequest> Photos { get; set; } = [];
}

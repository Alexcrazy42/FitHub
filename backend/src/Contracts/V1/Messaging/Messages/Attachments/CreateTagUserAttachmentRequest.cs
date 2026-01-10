using FitHub.Domain.Messaging.Attachments;

namespace FitHub.Contracts.V1.Messaging.Messages.Attachments;

public class CreateTagUserAttachmentRequest
{
    public string? Name { get; set; }

    public TagUserAttachmentType? Type { get; set; }

    public string? TaggedUserId { get; set; }
}

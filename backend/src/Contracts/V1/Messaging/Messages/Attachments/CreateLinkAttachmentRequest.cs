namespace FitHub.Contracts.V1.Messaging.Messages.Attachments;

public class CreateLinkAttachmentRequest
{
    public string? Url { get; set; }

    public string? Title { get; set; }

    public string? Caption { get; set; }

    public string? PhotoUrl { get; set; }
}

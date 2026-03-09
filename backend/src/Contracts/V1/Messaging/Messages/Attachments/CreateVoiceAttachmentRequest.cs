namespace FitHub.Contracts.V1.Messaging.Messages.Attachments;

public class CreateVoiceAttachmentRequest
{
    public string? FileId { get; set; }
    public int DurationMs { get; set; }
    public string? MimeType { get; set; }
    public List<float> Peaks { get; set; } = [];
}

using FitHub.Domain.Files;

namespace FitHub.Application.Messaging.Commands.Attachments;

public class CreateVoiceAttachmentCommand
{
    public FileId FileId { get; init; }
    public int DurationMs { get; init; }
    public string MimeType { get; init; }
    public float[] Peaks { get; init; }

    public CreateVoiceAttachmentCommand(FileId fileId, int durationMs, string mimeType, float[] peaks)
    {
        FileId = fileId;
        DurationMs = durationMs;
        MimeType = mimeType;
        Peaks = peaks;
    }
}

using FitHub.Domain.Files;

namespace FitHub.Domain.Messaging.Attachments;

public class VoiceAttachment
{
    public FileId FileId { get; set; }
    public int DurationMs { get; set; }
    public string MimeType { get; set; }
    public float[] Peaks { get; set; }

    public VoiceAttachment(FileId fileId, int durationMs, string mimeType, float[] peaks)
    {
        FileId = fileId;
        DurationMs = durationMs;
        MimeType = mimeType;
        Peaks = peaks;
    }
}

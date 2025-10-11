namespace FitHub.Contracts.V1.Files;

public class PresignedUrlResponse
{
    public string? FileId { get; set; }
    public string? Url { get; set; }
    public string? ObjectKey { get; set; }
}

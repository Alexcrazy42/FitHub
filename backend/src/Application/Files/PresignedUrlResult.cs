namespace FitHub.Application.Files;

public class PresignedUrlResult
{
    public required string FileId { get; init; }
    public required string Url { get; init; }
    public required string ObjectKey { get; init; }
}

namespace FitHub.Contracts.V1.Videos;

public record InitVideoUploadRequest(string? Title, string? FileExtension);

public record InitVideoMultipartUploadRequest(string? Title, string? FileExtension, long? FileSizeBytes);

public record CompleteVideoMultipartUploadRequest(IReadOnlyList<MultipartPartETagRequest>? Parts);

public record MultipartPartETagRequest(int PartNumber, string ETag);

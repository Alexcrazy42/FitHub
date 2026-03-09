namespace FitHub.Application.Videos;

public interface IVideoStorageService
{
    /// <summary>Returns a short-lived presigned PUT URL for the browser to upload the raw video.</summary>
    Task<string> GetPresignedUploadUrlAsync(string s3Key, CancellationToken ct);

    /// <summary>Returns a short-lived presigned GET URL for browser playback.</summary>
    Task<string> GetPresignedDownloadUrlAsync(string s3Key, TimeSpan expiry, CancellationToken ct);

    /// <summary>Streams the object from MinIO (used by the encoding worker).</summary>
    Task<Stream> DownloadAsync(string s3Key, CancellationToken ct);

    /// <summary>Uploads a local file to MinIO (used by the encoding worker).</summary>
    Task UploadFileAsync(string s3Key, string localPath, string contentType, CancellationToken ct);

    Task DeleteAsync(string s3Key, CancellationToken ct);
}

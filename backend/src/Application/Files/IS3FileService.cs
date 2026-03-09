namespace FitHub.Application.Files;

public interface IS3FileService
{
    Task<string> UploadFileAsync(string key, Stream fileStream, string contentType);
    Task<Stream> DownloadFileAsync(string key);
    Task<bool> DeleteFileAsync(string key);
    Task<bool> FileExistsAsync(string key);
    Task EnsureBucketExistsAsync();

    Task<PresignedUrlResult> GetPresignedUrlAsync(string fileId, string s3Key);
    Task<string> GetPresignedDownloadUrlAsync(string s3Key, TimeSpan expiry);
}

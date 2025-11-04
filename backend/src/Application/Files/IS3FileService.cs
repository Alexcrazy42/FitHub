namespace FitHub.Application.Files;

public interface IS3FileService
{
    Task<string> UploadFileAsync(string key, Stream fileStream, string contentType);
    Task<Stream> DownloadFileAsync(string key);
    Task<bool> DeleteFileAsync(string key);
    Task<bool> FileExistsAsync(string key);
    Task EnsureBucketExistsAsync();

    Task<PresignedUrlResult> GetPresignedUrlAsync(GetPresignedUrlCommand command, string fileId, string s3Key);
}

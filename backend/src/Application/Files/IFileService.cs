namespace FitHub.Application.Files;

public interface IFileService
{
    Task<string> UploadFileAsync(string key, Stream fileStream, string contentType);
    Task<Stream> DownloadFileAsync(string key);
    Task<bool> DeleteFileAsync(string key);
    Task<bool> FileExistsAsync(string key);
    Task EnsureBucketExistsAsync();
}

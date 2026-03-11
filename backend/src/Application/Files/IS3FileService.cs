namespace FitHub.Application.Files;

public record S3MultipartPart(int PartNumber, string ETag);

public interface IS3FileService
{
    Task<string> UploadFileAsync(string key, Stream fileStream, string contentType);
    Task<Stream> DownloadFileAsync(string key);
    Task<bool> DeleteFileAsync(string key);
    Task<bool> FileExistsAsync(string key);
    Task EnsureBucketExistsAsync();

    Task<PresignedUrlResult> GetPresignedUrlAsync(string fileId, string s3Key);
    Task<string> GetPresignedDownloadUrlAsync(string s3Key, TimeSpan expiry);

    Task<string> InitiateMultipartUploadAsync(string s3Key, string contentType);
    Task<string> GetPresignedPartUrlAsync(string s3Key, string uploadId, int partNumber);
    Task CompleteMultipartUploadAsync(string s3Key, string uploadId, IReadOnlyList<S3MultipartPart> parts);
    Task AbortMultipartUploadAsync(string s3Key, string uploadId);
}

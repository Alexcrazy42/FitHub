using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;

namespace FitHub.Application.Videos;

public class VideoStorageService : IVideoStorageService
{
    private readonly IAmazonS3 _s3;
    private readonly string _bucket;

    public VideoStorageService(IAmazonS3 s3, IConfiguration configuration)
    {
        _s3 = s3;
        _bucket = configuration["AWS:BucketName"]
                  ?? throw new InvalidOperationException("AWS:BucketName is not configured.");
    }

    public async Task<string> GetPresignedUploadUrlAsync(string s3Key, CancellationToken ct)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucket,
            Key = s3Key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(30),
            Protocol = Protocol.HTTP,
        };
        return await _s3.GetPreSignedURLAsync(request);
    }

    public async Task<string> GetPresignedDownloadUrlAsync(string s3Key, TimeSpan expiry, CancellationToken ct)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucket,
            Key = s3Key,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.Add(expiry),
            Protocol = Protocol.HTTP,
        };
        return await _s3.GetPreSignedURLAsync(request);
    }

    public async Task<Stream> DownloadAsync(string s3Key, CancellationToken ct)
    {
        var response = await _s3.GetObjectAsync(new GetObjectRequest
        {
            BucketName = _bucket,
            Key = s3Key,
        }, ct);
        return response.ResponseStream;
    }

    public async Task UploadFileAsync(string s3Key, string localPath, string contentType, CancellationToken ct)
    {
        await using var stream = File.OpenRead(localPath);
        await _s3.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _bucket,
            Key = s3Key,
            InputStream = stream,
            ContentType = contentType,
        }, ct);
    }

    public async Task DeleteAsync(string s3Key, CancellationToken ct)
    {
        await _s3.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = _bucket,
            Key = s3Key,
        }, ct);
    }
}

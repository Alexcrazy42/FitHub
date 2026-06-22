using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FitHub.Application.Files;

public class S3FileService : IS3FileService
{
    private readonly IAmazonS3 internalS3Client;
    private readonly IAmazonS3 publicS3Client;
    private readonly string bucketName;
    private readonly bool needToEnsureBucketExists;
    private readonly ILogger<S3FileService> logger;
    private readonly string serviceUrl;
    private readonly string publicUrl;

    public S3FileService(
        [FromKeyedServices("internal")] IAmazonS3 internalS3Client,
        [FromKeyedServices("public")] IAmazonS3 publicS3Client,
        IConfiguration configuration,
        ILogger<S3FileService> logger)
    {
        this.internalS3Client = internalS3Client;
        this.publicS3Client = publicS3Client;
        this.logger = logger;
        bucketName = configuration["AWS:BucketName"]
                     ?? throw new ArgumentException("BucketName is not configured.");

        needToEnsureBucketExists =
            Boolean.Parse(configuration["AWS:NeedToEnsureBucketExists"] ?? throw new Exception("AWS:NeedToEnsureBucketExists is null"));

        serviceUrl = configuration["AWS:ServiceUrl"] ?? throw new ArgumentException("ServiceUrl is not configured.");
        publicUrl = configuration["AWS:PublicURL"] ?? throw new ArgumentException("AWS:PublicUrl is not configured.");
    }

    public async Task EnsureBucketExistsAsync()
    {
        if (!needToEnsureBucketExists)
        {
            return;
        }

        try
        {
            var request = new GetBucketLocationRequest { BucketName = bucketName };
            await internalS3Client.GetBucketLocationAsync(request);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            var bucketRequest = new PutBucketRequest { BucketName = bucketName, UseClientRegion = true };
            await internalS3Client.PutBucketAsync(bucketRequest);
            logger.LogDebug("Bucket {BucketName} created", bucketName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking or creating bucket");
            throw;
        }
    }

    public async Task<PresignedUrlResult> GetPresignedUrlAsync(string fileId, string s3Key)
    {
        var presignRequest = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = s3Key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(15),
            Protocol = Protocol.HTTP
        };
        var url = await publicS3Client.GetPreSignedURLAsync(presignRequest);

        return new PresignedUrlResult { Url = url, FileId = fileId, ObjectKey = s3Key };
    }

    public async Task<string> GetPresignedDownloadUrlAsync(string s3Key, TimeSpan expiry)
    {
        var presignRequest = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = s3Key,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.Add(expiry),
            Protocol = Protocol.HTTP
        };
        var url = await publicS3Client.GetPreSignedURLAsync(presignRequest);
        return url;
    }

    public async Task<string> UploadFileAsync(string key, Stream fileStream, string contentType)
    {
        await EnsureBucketExistsAsync();
        var request = new PutObjectRequest { BucketName = bucketName, Key = key, InputStream = fileStream, ContentType = contentType };

        var response = await internalS3Client.PutObjectAsync(request);
        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            return key;
        }

        throw new Exception($"Failed to upload file: {response.HttpStatusCode}");
    }

    public async Task<Stream> DownloadFileAsync(string key)
    {
        await EnsureBucketExistsAsync();
        if (!await FileExistsAsync(key))
        {
            throw new FileNotFoundException($"File with key '{key}' not found.");
        }

        var request = new GetObjectRequest { BucketName = bucketName, Key = key };

        var response = await internalS3Client.GetObjectAsync(request);
        return response.ResponseStream;
    }

    public async Task<bool> DeleteFileAsync(string key)
    {
        await EnsureBucketExistsAsync();
        if (!await FileExistsAsync(key))
        {
            return false;
        }

        var request = new DeleteObjectRequest { BucketName = bucketName, Key = key };

        var response = await internalS3Client.DeleteObjectAsync(request);
        return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
    }

    public async Task<bool> FileExistsAsync(string key)
    {
        await EnsureBucketExistsAsync();
        try
        {
            var request = new GetObjectMetadataRequest { BucketName = bucketName, Key = key };
            await internalS3Client.GetObjectMetadataAsync(request);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task<string> InitiateMultipartUploadAsync(string s3Key, string contentType)
    {
        await EnsureBucketExistsAsync();
        var request = new InitiateMultipartUploadRequest { BucketName = bucketName, Key = s3Key, ContentType = contentType, };
        var response = await internalS3Client.InitiateMultipartUploadAsync(request);
        return response.UploadId;
    }

    public Task<string> GetPresignedPartUrlAsync(string s3Key, string uploadId, int partNumber)
    {
        var presignRequest = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = s3Key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddHours(2),
            Protocol = Protocol.HTTP,
            UploadId = uploadId,
            PartNumber = partNumber,
        };
        return publicS3Client.GetPreSignedURLAsync(presignRequest);
    }

    public async Task CompleteMultipartUploadAsync(string s3Key, string uploadId, IReadOnlyList<S3MultipartPart> parts)
    {
        var request = new CompleteMultipartUploadRequest
        {
            BucketName = bucketName,
            Key = s3Key,
            UploadId = uploadId,
            PartETags = parts.Select(p => new PartETag(p.PartNumber, p.ETag.Trim('"'))).ToList(),
        };
        await internalS3Client.CompleteMultipartUploadAsync(request);
    }

    public async Task AbortMultipartUploadAsync(string s3Key, string uploadId)
    {
        var request = new AbortMultipartUploadRequest { BucketName = bucketName, Key = s3Key, UploadId = uploadId, };
        await internalS3Client.AbortMultipartUploadAsync(request);
    }
}

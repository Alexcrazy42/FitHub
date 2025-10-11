using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;

namespace FitHub.Application.Files;

public class S3FileService : IS3FileService
{
    private readonly IAmazonS3 s3Client;
    private readonly string bucketName;
    private readonly bool needToEnsureBucketExists;

    public S3FileService(IAmazonS3 s3Client, IConfiguration configuration)
    {
        this.s3Client = s3Client;
        bucketName = configuration["AWS:BucketName"]
                      ?? throw new ArgumentException("BucketName is not configured.");

        needToEnsureBucketExists = bool.Parse(configuration["AWS:NeedToEnsureBucketExists"] ?? throw new Exception(""));
    }

    public async Task EnsureBucketExistsAsync()
    {
        if (!needToEnsureBucketExists)
        {
            return;
        }
        try
        {
            var request = new GetBucketLocationRequest
            {
                BucketName = bucketName
            };
            await s3Client.GetBucketLocationAsync(request);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            var bucketRequest = new PutBucketRequest
            {
                BucketName = bucketName,
                UseClientRegion = true
            };
            await s3Client.PutBucketAsync(bucketRequest);
            Console.WriteLine($"Bucket '{bucketName}' created.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking or creating bucket: {ex.Message}");
            throw;
        }
    }

    public async Task<PresignedUrlResult> GetPresignedUrlAsync(GetPresignedUrlCommand command, string fileId, string s3Key)
    {
        var file = command.File;
        var presignRequest = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = s3Key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(15),
            ContentType = GetMimeType(file),
            Protocol = Protocol.HTTP
        };
        var url = await s3Client.GetPreSignedURLAsync(presignRequest);

        return new PresignedUrlResult
        {
            Url = url,
            FileId = fileId,
            ObjectKey = s3Key
        };
    }

    public async Task<string> UploadFileAsync(string key, Stream fileStream, string contentType)
    {
        await EnsureBucketExistsAsync();
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = contentType
        };

        var response = await s3Client.PutObjectAsync(request);
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

        var request = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = key
        };

        var response = await s3Client.GetObjectAsync(request);
        return response.ResponseStream;
    }

    public async Task<bool> DeleteFileAsync(string key)
    {
        await EnsureBucketExistsAsync();
        if (!await FileExistsAsync(key))
        {
            return false;
        }

        var request = new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = key
        };

        var response = await s3Client.DeleteObjectAsync(request);
        return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
    }

    public async Task<bool> FileExistsAsync(string key)
    {
        await EnsureBucketExistsAsync();
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = bucketName,
                Key = key
            };
            await s3Client.GetObjectMetadataAsync(request);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    private string GetMimeType(IFormFile file)
    {
        var provider = new FileExtensionContentTypeProvider();

        if (!provider.TryGetContentType(file.FileName, out var contentType))
        {
            // fallback если расширение неизвестно
            contentType = "application/octet-stream";
        }

        return contentType;
    }
}

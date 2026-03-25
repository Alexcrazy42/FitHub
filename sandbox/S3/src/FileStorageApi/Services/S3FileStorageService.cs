using Amazon.S3;
using Amazon.S3.Model;

namespace FileStorageApi.Services;

public class S3FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3FileStorageService(IAmazonS3 s3Client, IConfiguration configuration)
    {
        _s3Client = s3Client;
        _bucketName = configuration["AWS:BucketName"] 
                      ?? throw new ArgumentException("BucketName is not configured.");
    }

    public async Task EnsureBucketExistsAsync()
    {
        try
        {
            var request = new GetBucketLocationRequest
            {
                BucketName = _bucketName
            };
            await _s3Client.GetBucketLocationAsync(request);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            var bucketRequest = new PutBucketRequest
            {
                BucketName = _bucketName,
                UseClientRegion = true
            };
            await _s3Client.PutBucketAsync(bucketRequest);
            Console.WriteLine($"Bucket '{_bucketName}' created.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking or creating bucket: {ex.Message}");
            throw;
        }
    }

    public async Task<string> UploadFileAsync(string key, Stream fileStream, string contentType)
    {
        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = contentType
        };

        var response = await _s3Client.PutObjectAsync(request);
        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            return key;
        }

        throw new Exception($"Failed to upload file: {response.HttpStatusCode}");
    }

    public async Task<Stream> DownloadFileAsync(string key)
    {
        if (!await FileExistsAsync(key))
            throw new FileNotFoundException($"File with key '{key}' not found.");

        var request = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = key
        };

        var response = await _s3Client.GetObjectAsync(request);
        return response.ResponseStream;
    }

    public async Task<bool> DeleteFileAsync(string key)
    {
        if (!await FileExistsAsync(key)) return false;

        var request = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = key
        };

        var response = await _s3Client.DeleteObjectAsync(request);
        return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
    }

    public async Task<bool> FileExistsAsync(string key)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _bucketName,
                Key = key
            };
            await _s3Client.GetObjectMetadataAsync(request);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}
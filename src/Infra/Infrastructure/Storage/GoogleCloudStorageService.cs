using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;
using ApiTemplate.Application.Common.Interfaces;

namespace ApiTemplate.Infrastructure.Storage;

public class GoogleCloudStorageService : IStorageService
{
    private const string StorageBaseUrl = "https://storage.googleapis.com";

    private readonly StorageSettings _settings;
    private readonly StorageClient _client = StorageClient.Create();

    public GoogleCloudStorageService(IOptions<StorageSettings> settings)
    {
        _settings = settings.Value;
    }

    public string GetBaseUrlBucket(bool isPublic)
    {
        return StorageBaseUrl + "/" + (isPublic ? _settings.PublicBucketName : _settings.BucketName);
    }

    public async Task<string> GetFileAsync(
        bool isPublic,
        string fileName,
        CancellationToken cancellationToken = default
    )
    {
        var bucketName = isPublic ? _settings.PublicBucketName : _settings.BucketName;

        using var stream = new MemoryStream();
        await _client.DownloadObjectAsync(bucketName, fileName, stream, cancellationToken: cancellationToken);

        return Convert.ToBase64String(stream.ToArray());
    }

    public Task<string?> PutObjectAsync(
        bool isPublic,
        string fileName,
        string mimeType,
        string base64File,
        CancellationToken cancellationToken = default
    )
    {
        var binaryFile = Convert.FromBase64String(base64File);

        return PutObjectAsync(isPublic, fileName, mimeType, binaryFile, cancellationToken);
    }

    public async Task<string?> PutObjectAsync(
        bool isPublic,
        string fileName,
        string mimeType,
        byte[] binaryFile,
        CancellationToken cancellationToken = default
    )
    {
        var bucketName = isPublic ? _settings.PublicBucketName : _settings.BucketName;

        var response = await _client.UploadObjectAsync(
            bucketName,
            fileName,
            mimeType,
            new MemoryStream(binaryFile),
            null,
            cancellationToken
        );

        return response != null ? $"{StorageBaseUrl}/{bucketName}/{fileName}" : null;
    }

    public async Task DeleteObjectAsync(
        bool isPublic,
        string fileName,
        CancellationToken cancellationToken = default
    )
    {
        var bucketName = isPublic ? _settings.PublicBucketName : _settings.BucketName;

        try
        {
            await _client.DeleteObjectAsync(bucketName, fileName, null, cancellationToken);
        }
        catch (Exception)
        {
            // TODO we may need to specifically capture the (not found) exception
        }
    }
}

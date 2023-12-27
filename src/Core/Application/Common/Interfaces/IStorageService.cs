namespace ApiTemplate.Application.Common.Interfaces;

public interface IStorageService : IScopedService
{
    string GetBaseUrlBucket(bool isPublic);

    Task<string> GetFileAsync(bool isPublic, string fileName, CancellationToken cancellationToken = default);

    Task<string?> PutObjectAsync(
        bool isPublic,
        string fileName,
        string mimeType,
        string base64File,
        CancellationToken cancellationToken = default
    );

    Task<string?> PutObjectAsync(
        bool isPublic,
        string fileName,
        string mimeType,
        byte[] binaryFile,
        CancellationToken cancellationToken = default
    );

    Task DeleteObjectAsync(bool isPublic, string fileName, CancellationToken cancellationToken = default);
}

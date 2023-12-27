using File = ApiTemplate.Domain.Entities.File;

namespace ApiTemplate.Application.Common.Services.Files;

public class FileService : IScopedService
{
    private readonly IRepository<File> _fileRepository;
    private readonly IStorageService _storageService;

    public FileService(IRepository<File> fileRepository, IStorageService storageService)
    {
        _fileRepository = fileRepository;
        _storageService = storageService;
    }

    private async Task<(File file, string? fileUrl)> UploadAsync(
        Guid? newFileId,
        BaseFileRequest request,
        bool isPublic,
        IFileFolder fileFolder,
        CancellationToken cancellationToken = default
    )
    {
        var binaryFile = Convert.FromBase64String(request.Base64);

        var file = new File()
        {
            Id = newFileId ?? Ulid.NewGuid(),
            Name = request.Name,
            Mime = request.Mime,
            Size = binaryFile.Length,
            Public = isPublic,
        };

        _fileRepository.Add(file);

        var filepath = $"{fileFolder.Path}/{file.Id}{Path.GetExtension(file.Name)}";
        var fileUrl = await _storageService.PutObjectAsync(isPublic, filepath, file.Mime, binaryFile,
            cancellationToken);

        file.Src = filepath;

        return (file, fileUrl);
    }

    public Task<(File file, string? fileUrl)> UploadNewAsync(
        Guid newFileId,
        BaseFileRequest request,
        bool isPublic,
        IFileFolder fileFolder,
        CancellationToken cancellationToken = default
    )
    {
        return UploadAsync(newFileId, request, isPublic, fileFolder, cancellationToken);
    }

    public Task<(File file, string? fileUrl)> UploadNewAsync(
        BaseFileRequest request,
        bool isPublic,
        IFileFolder fileFolder,
        CancellationToken cancellationToken = default
    )
    {
        return UploadAsync(null, request, isPublic, fileFolder, cancellationToken);
    }

    public async Task<(File file, string? fileUrl)> DeleteAndUploadNewAsync(
        Guid? fileIdToDelete,
        BaseFileRequest request,
        bool isPublic,
        IFileFolder fileFolder,
        CancellationToken cancellationToken = default
    )
    {
        if (fileIdToDelete != null)
        {
            var fileToDelete = await _fileRepository.GetByIdAsync((Guid)fileIdToDelete, cancellationToken)
                               ?? throw new NotFoundException(
                                   ErrorCodes.FileNotFound,
                                   $"File with id '{fileIdToDelete}' was not found"
                               );

            fileToDelete.MarkForDeletion = true;
        }

        return await UploadNewAsync(request, isPublic, fileFolder, cancellationToken);
    }

    /// <summary>
    /// Replace the File content of an existing file
    /// </summary>
    public async Task<(File file, string? fileUrl)> ReplaceAsync(
        Guid fileId,
        BaseFileRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var file = await _fileRepository.GetByIdAsync(fileId, cancellationToken)
                   ?? throw new NotFoundException(
                       ErrorCodes.FileNotFound,
                       $"File with id '{fileId}' was not found"
                   );

        var binaryFile = Convert.FromBase64String(request.Base64);

        file.Name = request.Name;
        file.Mime = request.Mime;
        file.Size = binaryFile.Length;
        file.Src = Path.ChangeExtension(file.Src, Path.GetExtension(file.Name));

        var fileUrl = await _storageService.PutObjectAsync(file.Public, file.Src, file.Mime, binaryFile,
            cancellationToken);

        return (file, fileUrl);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var file = await _fileRepository.GetByIdAsync(id, cancellationToken);

        if (file == null)
            return;

        if (!string.IsNullOrEmpty(file.Src))
        {
            await _storageService.DeleteObjectAsync(file.Public, file.Src, cancellationToken);
        }

        _fileRepository.Delete(file);
    }
}

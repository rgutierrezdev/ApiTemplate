using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ApiTemplate.Application.Common.Services.Files;
using File = ApiTemplate.Domain.Entities.File;

namespace ApiTemplate.Infrastructure.BackgroundJobs.Scheduled;

public class DeleteZombieFiles : IScheduledJob
{
    private readonly DbContext _dbContext;
    private readonly ILogger<DeleteZombieFiles> _logger;
    private readonly FileService _fileService;

    public DeleteZombieFiles(
        DbContext dbContext,
        ILogger<DeleteZombieFiles> logger,
        FileService fileService
    )
    {
        _dbContext = dbContext;
        _logger = logger;
        _fileService = fileService;
    }

    public async Task Invoke()
    {
        var fileIds = await _dbContext
            .Set<File>()
            .Where(f => f.MarkForDeletion)
            .Select(f => f.Id)
            .ToListAsync();

        var deleted = 0;
        var failed = 0;

        foreach (var fileId in fileIds)
        {
            try
            {
                await _fileService.DeleteAsync(fileId);
                await _dbContext.SaveChangesAsync();

                deleted++;
            }
            catch (Exception)
            {
                failed++;
            }
        }

        _logger.LogInformation(
            "Zombie files deleted: {DeletedCount}, failed to delete: {FailedCount}",
            deleted, failed
        );
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ApiTemplate.Domain.Entities.Identity;

namespace ApiTemplate.Infrastructure.BackgroundJobs.Scheduled;

public class DeleteExpiredBlacklistedTokens : IScheduledJob
{
    private readonly DbContext _dbContext;
    private readonly ILogger<DeleteExpiredBlacklistedTokens> _logger;

    public DeleteExpiredBlacklistedTokens(
        DbContext dbContext,
        ILogger<DeleteExpiredBlacklistedTokens> logger
    )
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Invoke()
    {
        var deleted = await _dbContext
            .Set<BlacklistedToken>()
            .Where(bt => bt.ExpiryDate <= DateTime.UtcNow)
            .ExecuteDeleteAsync();

        _logger.LogInformation("Blacklisted tokens deleted: {DeletedCount}", deleted);
    }
}

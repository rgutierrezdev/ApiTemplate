using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ApiTemplate.Application.Common.Events;
using ApiTemplate.Application.Common.Interfaces;
using ApiTemplate.Persistence.Configurations;

namespace ApiTemplate.Persistence.Context;

public class ApplicationDbContext : BaseDbContext
{
    public ApplicationDbContext(
        DbContextOptions options,
        IOptions<DatabaseSettings> dbSettings,
        IEventPublisher eventPublisher,
        ISerializerService serializer,
        IEnumerable<EntityTypeConfigurationDependency> configurations,
        ICurrentUser currentUser
    )
        : base(options, dbSettings, eventPublisher, serializer, configurations, currentUser)
    {
    }
}

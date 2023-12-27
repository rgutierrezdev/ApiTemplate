using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ApiTemplate.Application.Common.Events;
using ApiTemplate.Application.Common.Interfaces;
using ApiTemplate.Domain.Common.Contracts;
using ApiTemplate.Domain.Common.Events;
using ApiTemplate.Domain.Entities.Identity;
using ApiTemplate.Persistence.Auditing;
using ApiTemplate.Persistence.Configurations;
using ApiTemplate.Persistence.Converters;

namespace ApiTemplate.Persistence.Context;

public abstract class BaseDbContext : DbContext
{
    private readonly IEventPublisher _eventPublisher;
    private readonly ISerializerService _serializer;
    private readonly IEnumerable<EntityTypeConfigurationDependency> _configurations;
    private readonly ICurrentUser _currentUser;

    protected BaseDbContext(
        DbContextOptions options,
        IOptions<DatabaseSettings> dbSettings,
        IEventPublisher eventPublisher,
        ISerializerService serializer,
        IEnumerable<EntityTypeConfigurationDependency> configurations,
        ICurrentUser currentUser
    ) : base(options)
    {
        _eventPublisher = eventPublisher;
        _serializer = serializer;
        _configurations = configurations;
        _currentUser = currentUser;

        ChangeTracker.LazyLoadingEnabled = false;
    }

    private DbSet<AuditTrail> AuditTrails => Set<AuditTrail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityTypeConfiguration in _configurations)
        {
            entityTypeConfiguration.Configure(modelBuilder);
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<DateOnly>()
            .HaveConversion<DateOnlyConverter>()
            .HaveColumnType("date");

        base.ConfigureConventions(builder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var trailMakers = HandleAuditingBeforeSaveChanges(_currentUser.IsAuthenticated ? _currentUser.Id : null);

        var entitiesWithEvents = ChangeTracker.Entries<IEntity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .ToArray();

        var result = await base.SaveChangesAsync(cancellationToken);

        await HandleAuditingAfterSaveChangesAsync(trailMakers, cancellationToken);

        await SendDomainEventsAsync(entitiesWithEvents);

        return result;
    }

    private List<AuditTrailMaker> HandleAuditingBeforeSaveChanges(Guid? userId)
    {
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedDate = DateTime.UtcNow;
                    break;
            }
        }

        ChangeTracker.DetectChanges();

        foreach (var entry in ChangeTracker.Entries().ToList())
        {
            var entity = entry.Entity;
            var actualType = entity.GetType();
            var ent = entity as IEntity;

            // this conversion is really important, please do not change it
            dynamic castedObject = Convert.ChangeType(entity, actualType);

            switch (entry.State)
            {
                case EntityState.Added:
                    ent!.DomainEvents.Add(EntityCreatedEvent.WithEntity(castedObject));
                    break;

                case EntityState.Modified:
                    ent!.DomainEvents.Add(EntityUpdatedEvent.WithEntity(castedObject));
                    break;

                case EntityState.Deleted:
                    ent!.DomainEvents.Add(EntityDeletedEvent.WithEntity(castedObject));
                    break;
            }
        }

        var trailMakers = new List<AuditTrailMaker>();
        var changedEntries = ChangeTracker.Entries<IAuditableEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        foreach (var entry in changedEntries)
        {
            var trailMaker = new AuditTrailMaker(_serializer)
            {
                TableName = entry.Entity.GetType().Name,
                UserId = userId
            };

            trailMaker.TrailType = entry.State switch
            {
                EntityState.Added => TrailType.Create,
                EntityState.Deleted => TrailType.Delete,
                EntityState.Modified => TrailType.Update,
                _ => trailMaker.TrailType
            };

            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary)
                {
                    trailMaker.TemporaryProperties.Add(property);
                    continue;
                }

                var propertyName = property.Metadata.Name;

                if (property.Metadata.IsPrimaryKey())
                {
                    trailMaker.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        trailMaker.NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        trailMaker.OldValues[propertyName] = property.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified && property.OriginalValue?.Equals(property.CurrentValue) == false)
                        {
                            trailMaker.ChangedColumns.Add(propertyName);
                            trailMaker.OldValues[propertyName] = property.OriginalValue;
                            trailMaker.NewValues[propertyName] = property.CurrentValue;
                        }

                        break;
                }
            }

            trailMakers.Add(trailMaker);
        }

        foreach (var trailMaker in trailMakers.Where(e => !e.HasTemporaryProperties))
        {
            AuditTrails.Add(trailMaker.ToAuditTrail());
        }

        return trailMakers.Where(e => e.HasTemporaryProperties).ToList();
    }

    private Task HandleAuditingAfterSaveChangesAsync(
        List<AuditTrailMaker> trailMakers,
        CancellationToken cancellationToken = default
    )
    {
        if (trailMakers.Count == 0)
        {
            return Task.CompletedTask;
        }

        foreach (var trailMaker in trailMakers)
        {
            foreach (var prop in trailMaker.TemporaryProperties)
            {
                if (prop.Metadata.IsPrimaryKey())
                {
                    trailMaker.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                }
                else
                {
                    trailMaker.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                }
            }

            AuditTrails.Add(trailMaker.ToAuditTrail());
        }

        return SaveChangesAsync(cancellationToken);
    }

    private async Task SendDomainEventsAsync(IEnumerable<IEntity> entities)
    {
        foreach (var entity in entities)
        {
            var domainEvents = entity.DomainEvents.ToArray();
            entity.DomainEvents.Clear();

            foreach (var domainEvent in domainEvents)
            {
                await _eventPublisher.PublishAsync(domainEvent);
            }
        }
    }
}

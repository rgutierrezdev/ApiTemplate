using System.ComponentModel.DataAnnotations.Schema;

namespace ApiTemplate.Domain.Common.Contracts;

public abstract class BaseEntity : BaseEntity<Guid>
{
}

public abstract class BaseEntity<TId> : IEntity<TId>
{
    public TId Id { get; set; } = default!;

    [NotMapped]
    public List<DomainEvent> DomainEvents { get; } = new();
}

public abstract class BaseWeakEntity : IEntity
{
    [NotMapped]
    public List<DomainEvent> DomainEvents { get; } = new();
}

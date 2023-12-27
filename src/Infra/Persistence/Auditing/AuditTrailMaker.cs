using Microsoft.EntityFrameworkCore.ChangeTracking;
using ApiTemplate.Application.Common.Interfaces;
using ApiTemplate.Domain.Common;
using ApiTemplate.Domain.Entities.Identity;

namespace ApiTemplate.Persistence.Auditing;

public class AuditTrailMaker
{
    private readonly ISerializerService _serializer;

    public AuditTrailMaker(ISerializerService serializer)
    {
        _serializer = serializer;
    }

    public Guid? UserId { get; set; }
    public string TableName { get; set; } = default!;
    public Dictionary<string, object?> KeyValues { get; } = new();
    public Dictionary<string, object?> OldValues { get; } = new();
    public Dictionary<string, object?> NewValues { get; } = new();
    public List<PropertyEntry> TemporaryProperties { get; } = new();
    public TrailType TrailType { get; set; }
    public List<string> ChangedColumns { get; } = new();
    public bool HasTemporaryProperties => TemporaryProperties.Count > 0;

    public AuditTrail ToAuditTrail() =>
        new()
        {
            Id = Ulid.NewGuid(),
            UserId = UserId,
            Type = TrailType.ToString(),
            TableName = TableName,
            PrimaryKey = _serializer.Serialize(KeyValues),
            OldValues = OldValues.Count == 0 ? null : _serializer.Serialize(OldValues),
            NewValues = NewValues.Count == 0 ? null : _serializer.Serialize(NewValues),
            AffectedColumns = ChangedColumns.Count == 0 ? null : _serializer.Serialize(ChangedColumns),
            CreatedDate = DateTime.UtcNow,
        };
}

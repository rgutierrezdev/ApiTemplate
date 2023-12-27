namespace ApiTemplate.Domain.Entities.Identity;

public class AuditTrail : BaseEntity
{
    public string Type { get; set; } = default!;
    public Guid? UserId { get; set; }
    public string TableName { get; set; } = default!;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AffectedColumns { get; set; }
    public string? PrimaryKey { get; set; }
    public DateTime CreatedDate { get; set; }

    // Navigation properties
    public User? User { get; set; }
}

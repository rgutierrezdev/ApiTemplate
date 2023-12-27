namespace ApiTemplate.Domain.Common.Contracts;

public interface IAuditableEntity
{
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

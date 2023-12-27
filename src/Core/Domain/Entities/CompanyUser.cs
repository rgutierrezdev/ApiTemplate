using ApiTemplate.Domain.Entities.Customers;
using ApiTemplate.Domain.Entities.Identity;

namespace ApiTemplate.Domain.Entities;

public class CompanyUser : AuditableEntity
{
    public Guid CompanyId { get; set; }
    public Guid UserId { get; set; }
    public bool Owner { get; set; }
    public DateTime? DeletedDate { get; set; }

    // Navigation properties
    public Company Company { get; set; } = default!;
    public User User { get; set; } = default!;
    public ICollection<CostCenterUser> CostCenterUsers { get; set; } = default!;
    public ICollection<CompanyUserPermission> CompanyUserPermissions { get; set; } = default!;
    public ICollection<CouponCompanyUser> CouponCompanyUsers { get; set; } = default!;
}

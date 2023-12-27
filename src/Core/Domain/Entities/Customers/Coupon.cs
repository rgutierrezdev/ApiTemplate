using ApiTemplate.Domain.Entities.Identity;

namespace ApiTemplate.Domain.Entities.Customers;

public class Coupon : AuditableEntity
{
    public string Code { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal DiscountPercent { get; set; }
    public int Duration { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool SingleUse { get; set; }
    public Guid CreatedByUserId { get; set; }

    // Navigation properties
    public User CreatedByUser { get; set; } = default!;
    public ICollection<CouponCompanyUser> CouponCompanyUsers { get; set; } = default!;
}

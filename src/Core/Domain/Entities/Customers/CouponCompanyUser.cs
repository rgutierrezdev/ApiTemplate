namespace ApiTemplate.Domain.Entities.Customers;

public enum CouponCompanyUserStatus
{
    Reserved = 1,
    Applied = 2,
}

public class CouponCompanyUser : AuditableEntity
{
    public Guid CouponId { get; set; }
    public Guid CompanyUserId { get; set; }
    public CouponCompanyUserStatus Status { get; set; }

    // Navigation properties
    public Coupon Coupon { get; set; } = default!;
    public CompanyUser CompanyUser { get; set; } = default!;
}

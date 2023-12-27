namespace ApiTemplate.Application.Features.Coupons.Dtos;

public class CouponDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal DiscountPercent { get; set; }
    public int Duration { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool SingleUse { get; set; }
    public int UsedCount { get; set; }
}

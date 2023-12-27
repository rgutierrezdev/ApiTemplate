namespace ApiTemplate.Application.Features.Coupons.Dtos;

public class PageCouponDto : CouponDto
{
    public DateTime CreatedDate { get; set; }
    public string CreatedByUser { get; set; } = default!;
}

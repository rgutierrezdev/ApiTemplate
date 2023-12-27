namespace ApiTemplate.Application.Features.Companies.Onboarding.Dtos;

public class CompanyCouponDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public decimal DiscountPercent { get; set; }
    public int Duration { get; set; }
}

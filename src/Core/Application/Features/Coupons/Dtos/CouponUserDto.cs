using ApiTemplate.Domain.Entities.Customers;

namespace ApiTemplate.Application.Features.Coupons.Dtos;

public class CouponUserDto
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = default!;
    public string UserFullName { get; set; } = default!;
    public CouponCompanyUserStatus Status { get; set; }
}

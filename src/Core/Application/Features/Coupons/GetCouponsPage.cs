using System.Linq.Expressions;
using ApiTemplate.Application.Features.Coupons.Dtos;
using ApiTemplate.Domain.Entities.Customers;

namespace ApiTemplate.Application.Features.Coupons;

public class GetCouponsPage
{
    public enum StatusFilter
    {
        Active = 1,
        Expired = 2
    }

    public class Filters
    {
        public StatusFilter? Status { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public Guid? CompanyId { get; set; }
    }

    public enum OrderByFields
    {
        SingleUse,
        Description,
        CreatedDate,
        ExpiryDate,
        CreatedBy,
        Duration,
    }

    public class Request : PaginationRequest<Filters, OrderByFields>, IRequest<PaginationResponse<PageCouponDto>>
    {
    }

    internal class Handler : IRequestHandler<Request, PaginationResponse<PageCouponDto>>
    {
        private readonly IRepository<Coupon> _couponRepository;

        public Handler(IRepository<Coupon> couponRepository)
        {
            _couponRepository = couponRepository;
        }

        public async Task<PaginationResponse<PageCouponDto>> Handle(
            Request request,
            CancellationToken cancellationToken
        )
        {
            var spec = new PaginationSpecification<Coupon, PageCouponDto, Filters, OrderByFields>(
                request,
                query =>
                {
                    query
                        .Select(c => new PageCouponDto()
                        {
                            Id = c.Id,
                            Code = c.Code,
                            Description = c.Description,
                            Duration = c.Duration,
                            DiscountPercent = c.DiscountPercent,
                            ExpiryDate = c.ExpiryDate,
                            SingleUse = c.SingleUse,
                            UsedCount = c.CouponCompanyUsers.Count,
                            CreatedByUser = c.CreatedByUser.FirstName + " " + c.CreatedByUser.LastName,
                            CreatedDate = c.CreatedDate
                        })
                        .OrderByDescending(dt => dt.CreatedDate, !request.HasOrderBy());

                    request
                        .HandleFilter(Like.Scape, value =>
                        {
                            query
                                .Search(c => c.Code, value)
                                .Search(c => c.Description, value);
                        })
                        .HandleFilter(f => f.Status, value =>
                        {
                            switch (value)
                            {
                                case StatusFilter.Active:
                                    query.Where(c => c.ExpiryDate >= DateTime.UtcNow);
                                    break;
                                case StatusFilter.Expired:
                                    query.Where(c => c.ExpiryDate < DateTime.UtcNow);
                                    break;
                            }
                        })
                        .HandleFilter(f => f.CreatedByUserId, value =>
                        {
                            query
                                .Where(c => c.CreatedByUserId == value);
                        })
                        .HandleFilter(f => f.CompanyId, value =>
                        {
                            query
                                .Where(c => c.CouponCompanyUsers.Any(ccu => ccu.CompanyUser.CompanyId == value));
                        });
                },
                new Dictionary<OrderByFields, Expression<Func<Coupon, object?>>>()
                {
                    { OrderByFields.CreatedBy, c => c.CreatedByUser.FirstName + " " + c.CreatedByUser.LastName },
                }
            );

            var list = await _couponRepository.ListAsync(spec, cancellationToken);
            var total = await _couponRepository.CountAsync(spec, cancellationToken);

            return new PaginationResponse<PageCouponDto>(list, total);
        }
    }
}

using ApiTemplate.Application.Features.Coupons.Dtos;
using ApiTemplate.Domain.Entities.Customers;

namespace ApiTemplate.Application.Features.Coupons;

public class GetCoupon
{
    public class Request : IRequest<CouponDto>
    {
        public Guid Id { get; }

        public Request(Guid id)
        {
            Id = id;
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(p => p.Id)
                .NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, CouponDto>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Coupon> _repository;

        public Handler(IValidator<Request> validator, IRepository<Coupon> repository)
        {
            _validator = validator;
            _repository = repository;
        }

        public async Task<CouponDto> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var coupon = await _repository.FirstOrDefaultAsync<CouponDto>(
                query => query
                    .Select(c => new CouponDto()
                    {
                        Id = c.Id,
                        Code = c.Code,
                        Description = c.Description,
                        Duration = c.Duration,
                        DiscountPercent = c.DiscountPercent,
                        ExpiryDate = c.ExpiryDate,
                        SingleUse = c.SingleUse,
                        UsedCount = c.CouponCompanyUsers.Count,
                    })
                    .Where(dt => dt.Id == request.Id),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.CouponNotFound,
                $"Coupon with id '{request.Id}' was not found"
            );

            return coupon;
        }
    }
}

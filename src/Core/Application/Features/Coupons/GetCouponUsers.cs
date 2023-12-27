using ApiTemplate.Application.Features.Coupons.Dtos;
using ApiTemplate.Domain.Entities.Customers;

namespace ApiTemplate.Application.Features.Coupons;

public class GetCouponUsers
{
    public class Request : IRequest<List<CouponUserDto>>
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

    internal class Handler : IRequestHandler<Request, List<CouponUserDto>>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<CouponCompanyUser> _repository;

        public Handler(IValidator<Request> validator, IRepository<CouponCompanyUser> repository)
        {
            _validator = validator;
            _repository = repository;
        }

        public async Task<List<CouponUserDto>> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var coupon = await _repository.ListAsync<CouponUserDto>(
                query => query
                    .Select(ccu => new CouponUserDto()
                    {
                        Id = ccu.Id,
                        CompanyName = ccu.CompanyUser.Company.Name,
                        UserFullName = ccu.CompanyUser.User.FirstName + " " + ccu.CompanyUser.User.LastName,
                        Status = ccu.Status,
                    })
                    .Where(ccu => ccu.CouponId == request.Id),
                cancellationToken
            );

            return coupon;
        }
    }
}

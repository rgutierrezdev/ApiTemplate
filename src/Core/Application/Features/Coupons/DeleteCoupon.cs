using ApiTemplate.Domain.Entities.Customers;

namespace ApiTemplate.Application.Features.Coupons;

public class DeleteCoupon
{
    public class Request : IRequest<Guid>
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

    internal class Handler : IRequestHandler<Request, Guid>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Coupon> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IValidator<Request> validator, IRepository<Coupon> repository, IUnitOfWork unitOfWork)
        {
            _validator = validator;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var coupon = await _repository.FirstOrDefaultAsync(
                query => query
                    .Include(c => c.CouponCompanyUsers)
                    .Where(c => c.Id == request.Id),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.CouponNotFound,
                $"Coupon with id '{request.Id}' was not found"
            );

            if (coupon.CouponCompanyUsers.Count > 0)
            {
                throw new InvalidRequestException(
                    ErrorCodes.CouponAlreadyApplied,
                    $"Coupon with id '{request.Id}' is already reserved/applied"
                );
            }

            _repository.Delete(coupon);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return request.Id;
        }
    }
}

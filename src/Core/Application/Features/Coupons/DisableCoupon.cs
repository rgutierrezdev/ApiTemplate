using ApiTemplate.Domain.Entities.Customers;

namespace ApiTemplate.Application.Features.Coupons;

public class DisableCoupon
{
    public class Request : IRequest<Guid>
    {
        public Guid Id { get; set; }

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
        private readonly IRepository<Coupon> _couponRepository;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(
            IValidator<Request> validator,
            IRepository<Coupon> couponRepository,
            IUnitOfWork unitOfWork
        )
        {
            _validator = validator;
            _couponRepository = couponRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var coupon = await _couponRepository.GetByIdAsync(request.Id, cancellationToken)
                         ??
                         throw new NotFoundException(
                             ErrorCodes.CouponNotFound,
                             $"Coupon with id '{request.Id}' was not found"
                         );

            coupon.ExpiryDate = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return coupon.Id;
        }
    }
}

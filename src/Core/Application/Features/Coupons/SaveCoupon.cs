using ApiTemplate.Domain.Entities.Customers;

namespace ApiTemplate.Application.Features.Coupons;

public class SaveCoupon
{
    public class Request : IRequest<Guid>
    {
        public Guid? Id { get; set; }
        public string Description { get; set; } = default!;
        public decimal DiscountPercent { get; set; }
        public int Duration { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool SingleUse { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(p => p.Description)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(p => p.DiscountPercent)
                .NotEmpty()
                .InclusiveBetween(1, 100);

            RuleFor(p => p.ExpiryDate)
                .NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, Guid>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Coupon> _couponRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public Handler(
            IValidator<Request> validator,
            IRepository<Coupon> couponRepository,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser
        )
        {
            _validator = validator;
            _couponRepository = couponRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            Coupon coupon;

            if (request.Id == null)
            {
                coupon = new Coupon()
                {
                    Id = Ulid.NewGuid(),
                    CreatedByUserId = _currentUser.Id,
                };

                _couponRepository.Add(coupon);
            }
            else
            {
                coupon = await _couponRepository.FirstOrDefaultAsync(query => query
                        .Include(c => c.CouponCompanyUsers
                            .Where(ccu => ccu.Status == CouponCompanyUserStatus.Applied)
                        )
                        .Where(c => c.Id == request.Id),
                    cancellationToken
                ) ?? throw new NotFoundException(
                    ErrorCodes.CouponNotFound,
                    $"Coupon with id '{request.Id}' was not found"
                );

                if (coupon.CouponCompanyUsers.Count > 0 && coupon.Duration != request.Duration)
                {
                    throw new InvalidRequestException(
                        ErrorCodes.CouponAlreadyApplied,
                        $"Coupon with id '{request.Id}' is already/applied, Duration cannot be changed"
                    );
                }
            }

            coupon.Code = GenerateCouponCode();
            coupon.Description = request.Description;
            coupon.ExpiryDate = request.ExpiryDate;
            coupon.DiscountPercent = request.DiscountPercent;
            coupon.SingleUse = request.SingleUse;
            coupon.Duration = request.Duration;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return coupon.Id;
        }

        private string GenerateCouponCode()
        {
            const int length = 10;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[length];
            var random = new Random();

            for (var i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }
    }
}

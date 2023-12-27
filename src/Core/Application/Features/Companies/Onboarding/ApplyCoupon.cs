using ApiTemplate.Application.Features.Companies.Onboarding.Dtos;
using ApiTemplate.Domain.Entities.Customers;

namespace ApiTemplate.Application.Features.Companies.Onboarding;

public class ApplyCoupon
{
    public class Request : BaseRequest, IRequest<CompanyCouponDto>
    {
        public Guid CompanyId { get; set; }
    }

    public class BaseRequest
    {
        public string CouponCode { get; set; } = default!;
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.CompanyId)
                .NotEmpty();

            Include(new BaseValidator());
        }
    }

    internal class BaseValidator : AbstractValidator<BaseRequest>
    {
        public BaseValidator()
        {
            RuleFor(r => r.CouponCode)
                .NotEmpty()
                .MaximumLength(10);
        }
    }

    internal class Handler : IRequestHandler<Request, CompanyCouponDto>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Coupon> _couponRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<CouponCompanyUser> _couponCompanyUserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public Handler(
            IValidator<Request> validator,
            IRepository<Coupon> couponRepository,
            IRepository<Company> companyRepository,
            IRepository<CouponCompanyUser> couponCompanyUserRepository,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser
        )
        {
            _validator = validator;
            _couponRepository = couponRepository;
            _companyRepository = companyRepository;
            _couponCompanyUserRepository = couponCompanyUserRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<CompanyCouponDto> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.CompanyId,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.GetByIdAsync(request.CompanyId, cancellationToken)
                          ??
                          throw new NotFoundException(
                              ErrorCodes.CompanyNotFound,
                              $"Company with id '{request.CompanyId}' was not found"
                          );

            if (company.Status != CompanyStatus.OnBoarding)
            {
                throw new InvalidRequestException(
                    ErrorCodes.CompanyInvalidStatus,
                    $"This company status is '{company.Status.ToString()}'"
                );
            }

            var coupon = await GetAndValidateCouponAsync(request, cancellationToken);
            var currentCompanyUser = _currentUser.Companies!.First(c => c.Id == request.CompanyId);

            coupon.CouponCompanyUsers.Add(new CouponCompanyUser()
            {
                Id = Ulid.NewGuid(),
                CompanyUserId = currentCompanyUser.CompanyUserId,
                Status = CouponCompanyUserStatus.Reserved,
            });

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return coupon.Adapt<CompanyCouponDto>();
        }

        private async Task<Coupon> GetAndValidateCouponAsync(Request request, CancellationToken cancellationToken)
        {
            var coupon = await _couponRepository.FirstOrDefaultAsync(query => query
                    .Include(c => c.CouponCompanyUsers.Take(1))
                    .Where(c => c.Code == request.CouponCode),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.CouponNotFound,
                $"Coupon with code '{request.CouponCode}' was not found"
            );

            var isCouponAlreadyAppliedToCompany = await _couponCompanyUserRepository.AnyAsync(query => query
                    .Where(ccu => ccu.CompanyUser.CompanyId == request.CompanyId),
                cancellationToken
            );

            if (isCouponAlreadyAppliedToCompany)
            {
                throw new InvalidRequestException(
                    ErrorCodes.CompanyHasCoupon,
                    $"This company already has a coupon applied/reserved"
                );
            }

            if (DateTime.UtcNow > coupon.ExpiryDate)
            {
                throw new InvalidRequestException(
                    ErrorCodes.CouponExpired,
                    $"This coupon is expired"
                );
            }

            if (coupon.SingleUse && coupon.CouponCompanyUsers.Count > 0)
            {
                throw new InvalidRequestException(
                    ErrorCodes.CouponAlreadyApplied,
                    $"This coupon is been already applied/reserved"
                );
            }

            return coupon;
        }
    }
}

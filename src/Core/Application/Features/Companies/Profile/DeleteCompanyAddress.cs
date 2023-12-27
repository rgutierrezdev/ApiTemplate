namespace ApiTemplate.Application.Features.Companies.Profile;

public class DeleteCompanyAddress
{
    public class Request : IRequest<Guid>
    {
        public Guid CompanyId { get; }
        public Guid CompanyAddressId { get; }

        public Request(Guid companyId, Guid companyAddressId)
        {
            CompanyAddressId = companyAddressId;
            CompanyId = companyId;
        }
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty();

            RuleFor(x => x.CompanyAddressId)
                .NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, Guid>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<CompanyAddress> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public Handler(
            IValidator<Request> validator,
            IRepository<CompanyAddress> repository,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser
        )
        {
            _validator = validator;
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.CompanyId,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var companyAddress = await _repository.GetByIdAsync(request.CompanyAddressId, cancellationToken)
                                 ?? throw new NotFoundException(
                                     ErrorCodes.CompanyAddressNotFound,
                                     $"Company Address with id '{request.CompanyAddressId}' was not found"
                                 );

            if (request.CompanyId != companyAddress.CompanyId)
            {
                throw new InvalidRequestException(ErrorCodes.ParamsMissMatch);
            }

            _repository.Delete(companyAddress);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return request.CompanyAddressId;
        }
    }
}

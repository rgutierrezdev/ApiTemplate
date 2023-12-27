namespace ApiTemplate.Application.Features.Companies.Profile;

public class SendCompanySignRegistrationEmail
{
    public class Request : IRequest
    {
        public Guid CompanyId { get; }

        public Request(Guid companyId) => CompanyId = companyId;
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.CompanyId)
                .NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly IUtilsService _utilsService;
        private readonly CompanyService _companyService;

        public Handler(
            IValidator<Request> validator,
            IRepository<Company> companyRepository,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser,
            IUtilsService utilsService,
            CompanyService companyService
        )
        {
            _validator = validator;
            _companyRepository = companyRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _utilsService = utilsService;
            _companyService = companyService;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.CompanyId,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync(query => query
                    .Include(c => c.CompanyUsers.Where(cu => cu.Owner))
                    .ThenInclude(cu => cu.User)
                    .Include(c => c.CompanySignedFiles
                        .Where(csf => csf.Type == CompanySignedFileType.Registration)
                        .Take(1)
                    )
                    .Where(c => c.Id == request.CompanyId),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.CompanyNotFound,
                $"Company with id '{request.CompanyId}' was not found"
            );

            var isRegistrationAlreadySigned = company.CompanySignedFiles.Count > 0;
            if (isRegistrationAlreadySigned)
            {
                throw new InvalidRequestException(
                    ErrorCodes.CompanyRegistrationAlreadySigned,
                    "This company registration is already signed"
                );
            }

            company.SignOnboardingToken = _utilsService.GenerateRandomPassword(10);

            string signerEmail;
            string signerFirstName;
            string signerLastName;

            if (company.PersonType == PersonType.Natural)
            {
                var owner = company.CompanyUsers.FirstOrDefault();
                if (owner == null)
                {
                    throw new NotFoundException(
                        ErrorCodes.CompanyOwnerNotFound,
                        "This company owner was not found"
                    );
                }

                signerEmail = owner.User.Email;
                signerFirstName = owner.User.FirstName;
                signerLastName = owner.User.LastName;
            }
            else
            {
                signerEmail = company.LegalRepresentativeEmail!;
                signerFirstName = company.LegalRepresentativeFirstName!;
                signerLastName = company.LegalRepresentativeLastName!;
            }

            await _companyService.SendSignRegistrationEmailAsync(
                company.Id,
                signerEmail,
                company.Name,
                signerFirstName,
                signerLastName,
                company.SignOnboardingToken!,
                cancellationToken
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

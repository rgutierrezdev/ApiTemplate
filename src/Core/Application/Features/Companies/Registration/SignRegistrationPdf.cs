namespace ApiTemplate.Application.Features.Companies.Registration;

public class SignRegistrationPdf
{
    public class Request : BaseRequest, IRequest
    {
        public Guid CompanyId { get; set; }
    }

    public class BaseRequest
    {
        public string Token { get; set; } = default!;
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty();

            Include(new BaseValidator());
        }
    }

    internal class BaseValidator : AbstractValidator<BaseRequest>
    {
        public BaseValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;
        private readonly SignCompanyRegistrationFileService _signCompanyRegistrationFileService;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(
            IValidator<Request> validator,
            IRepository<Company> companyRepository,
            SignCompanyRegistrationFileService signCompanyRegistrationFileService,
            IUnitOfWork unitOfWork
        )
        {
            _validator = validator;
            _companyRepository = companyRepository;
            _signCompanyRegistrationFileService = signCompanyRegistrationFileService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var company = await _companyRepository.FirstOrDefaultAsync(query => query
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

            if (!BCrypt.Net.BCrypt.Verify(company.SignOnboardingToken, request.Token))
            {
                throw new UnauthorizedException(
                    ErrorCodes.InvalidSignToken,
                    "Provided sign token is invalid"
                );
            }

            if (company.CompanySignedFiles.Count > 0)
            {
                throw new InvalidRequestException(
                    ErrorCodes.CompanyRegistrationAlreadySigned,
                    "This company registration is already signed"
                );
            }

            var signedFile = await _signCompanyRegistrationFileService.GenerateAndSignAsync(
                request.CompanyId,
                cancellationToken
            );

            company.CompanySignedFiles.Add(new CompanySignedFile()
            {
                Id = signedFile.Id,
                Type = CompanySignedFileType.Registration
            });

            company.SignOnboardingToken = null;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

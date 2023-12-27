namespace ApiTemplate.Application.Features.Companies.Registration;

public class GetRegistrationPdf
{
    public class Request : BaseRequest, IRequest<EncodedFileResponse>
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

    internal class Handler : IRequestHandler<Request, EncodedFileResponse>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;
        private readonly ICompanyRegistrationPdf _companyRegistrationPdf;

        public Handler(
            IValidator<Request> validator,
            IRepository<Company> companyRepository,
            ICompanyRegistrationPdf companyRegistrationPdf
        )
        {
            _validator = validator;
            _companyRepository = companyRepository;
            _companyRegistrationPdf = companyRegistrationPdf;
        }

        public async Task<EncodedFileResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var company = await _companyRepository.FirstOrDefaultAsync(
                new CompanyRegistrationByIdSpec(request.CompanyId),
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

            if (company.IsRegistrationSigned)
            {
                throw new InvalidRequestException(
                    ErrorCodes.CompanyRegistrationAlreadySigned,
                    "This company registration is already signed"
                );
            }

            var pdf = _companyRegistrationPdf.Generate(company);

            return new EncodedFileResponse(MimeTypes.Pdf, Convert.ToBase64String(pdf));
        }
    }
}

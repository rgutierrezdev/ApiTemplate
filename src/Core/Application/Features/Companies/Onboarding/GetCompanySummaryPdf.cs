namespace ApiTemplate.Application.Features.Companies.Onboarding;

public class GetCompanySummaryPdf
{
    public class Request : IRequest<EncodedFileResponse>
    {
        public Guid Id { get; set; }

        public Request(Guid id)
        {
            Id = id;
        }
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, EncodedFileResponse>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;
        private readonly ICurrentUser _currentUser;
        private readonly ICompanyRegistrationPdf _companyRegistrationPdf;

        public Handler(
            IValidator<Request> validator,
            IRepository<Company> companyRepository,
            ICurrentUser currentUser,
            ICompanyRegistrationPdf companyRegistrationPdf
        )
        {
            _validator = validator;
            _companyRepository = companyRepository;
            _currentUser = currentUser;
            _companyRegistrationPdf = companyRegistrationPdf;
        }

        public async Task<EncodedFileResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.Id,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync(
                new CompanyRegistrationByIdSpec(request.Id),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.CompanyNotFound,
                $"Company with id '{request.Id}' was not found"
            );

            var pdf = _companyRegistrationPdf.Generate(company);

            return new EncodedFileResponse(MimeTypes.Pdf, Convert.ToBase64String(pdf));
        }
    }
}

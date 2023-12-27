using ApiTemplate.Application.Features.Companies.Onboarding.Dtos;

namespace ApiTemplate.Application.Features.Companies.Registration;

public class GetRegistration
{
    public class Request : BaseRequest, IRequest<CompanySummaryDto>
    {
        public Guid CompanyId { get; set; }
    }

    public class BaseRequest
    {
        public string Token { get; set; } = default!;
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty();

            Include(new BaseValidator());
        }
    }

    public class BaseValidator : AbstractValidator<BaseRequest>
    {
        public BaseValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty();
        }
    }

    private class CompanySummaryData : CompanySummaryDto
    {
        public string SignOnboardingToken { get; set; } = default!;
        public bool IsRegistrationSigned { get; set; }
    }

    internal class Handler : IRequestHandler<Request, CompanySummaryDto>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;

        public Handler(IValidator<Request> validator, IRepository<Company> companyRepository)
        {
            _validator = validator;
            _companyRepository = companyRepository;
        }

        public async Task<CompanySummaryDto> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var company = await _companyRepository.FirstOrDefaultAsync<CompanySummaryData>(query => query
                    .Select(c => new CompanySummaryData()
                    {
                        LegalRepresentativeFirstName = c.LegalRepresentativeFirstName!,
                        LegalRepresentativeLastName = c.LegalRepresentativeLastName!,
                        LegalRepresentativeEmail = c.LegalRepresentativeEmail!,
                        LegalRepresentativeDocumentTypeShortName = c.LegalRepresentativeDocumentType!.ShortName,
                        LegalRepresentativeDocumentTypeName = c.LegalRepresentativeDocumentType!.Name,
                        LegalRepresentativeDocument = c.LegalRepresentativeDocument!,
                        SignOnboardingToken = c.SignOnboardingToken!,
                        IsRegistrationSigned = c.CompanySignedFiles.Any(csf =>
                            csf.Type == CompanySignedFileType.Registration
                        )
                    })
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

            if (company.IsRegistrationSigned)
            {
                throw new InvalidRequestException(
                    ErrorCodes.CompanyRegistrationAlreadySigned,
                    "This company registration is already signed"
                );
            }

            return company.Adapt<CompanySummaryDto>();
        }
    }
}

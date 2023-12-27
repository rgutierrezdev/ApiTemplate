using ApiTemplate.Application.Features.Companies.Profile.Dtos;

namespace ApiTemplate.Application.Features.Companies.Profile;

public class GetCompanyProfileAgreements
{
    public class Request : IRequest<CompanyProfileAgreementsDto>
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

    internal class Handler : IRequestHandler<Request, CompanyProfileAgreementsDto>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;
        private readonly ICurrentUser _currentUser;

        public Handler(
            IValidator<Request> validator,
            IRepository<Company> companyRepository,
            ICurrentUser currentUser
        )
        {
            _validator = validator;
            _companyRepository = companyRepository;
            _currentUser = currentUser;
        }

        public async Task<CompanyProfileAgreementsDto> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.Id,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync<CompanyProfileAgreementsDto>(
                query => query
                    .Select(c => new CompanyProfileAgreementsDto()
                    {
                        Current = new CompanyProfileAgreementsDto.CurrenChange()
                        {
                            Registration = c.CompanySignedFiles
                                .Where(csf => csf.Type == CompanySignedFileType.Registration)
                                .OrderByDescending(csf => csf.CreatedDate)
                                .Select(csf =>
                                    new CompanyProfileAgreementsDto.Registration()
                                    {
                                        Name = csf.SignedFile.Name,
                                        FileId = csf.SignedFile.Id,
                                    })
                                .FirstOrDefault()
                        },
                        CompanyInfoReviewStatus = c.CompanyChange!.CompanyInfoReviewStatus,
                        BillingTaxesReviewStatus = c.CompanyChange.BillingTaxesReviewStatus,
                        AssociatesReviewStatus = c.CompanyChange.AssociatesReviewStatus,
                        DocumentsReviewStatus = c.CompanyChange.DocumentsReviewStatus,
                        CreditReviewStatus = c.CompanyChange.CreditReviewStatus,
                    })
                    .Where(c => c.Id == request.Id),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.CompanyNotFound,
                $"Company with id '{request.Id}' was not found"
            );

            return company;
        }
    }
}

using ApiTemplate.Application.Features.Companies.Dtos;
using ApiTemplate.Application.Features.Companies.Onboarding.Dtos;

namespace ApiTemplate.Application.Features.Companies.Onboarding;

public class GetCompanyDocuments
{
    public class Request : IRequest<List<CompanyDocumentDto>>
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

    internal class Handler : IRequestHandler<Request, List<CompanyDocumentDto>>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<CompanyDocument> _companyDocumentRepository;
        private readonly ICurrentUser _currentUser;

        public Handler(
            IValidator<Request> validator,
            IRepository<Company> companyRepository,
            IRepository<CompanyDocument> companyDocumentRepository,
            ICurrentUser currentUser
        )
        {
            _validator = validator;
            _companyRepository = companyRepository;
            _companyDocumentRepository = companyDocumentRepository;
            _currentUser = currentUser;
        }

        public async Task<List<CompanyDocumentDto>> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.Id,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync(
                query => query
                    .Include(c => c.CompanyChange)
                    .Include(c => c.CompanyDocumentsFiles.Where(cdf => cdf.ChangeCompanyDocumentFileId != null))
                    .ThenInclude(cdf => cdf.File)
                    .Where(c => c.Id == request.Id),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.CompanyNotFound,
                $"Company with id '{request.Id}' was not found"
            );

            var isCompanyCreditEnabled = company.CompanyChange?.CreditEnabled ?? company.CreditEnabled;

            var companyDocuments = await _companyDocumentRepository.ListAsync<CompanyDocumentDto>(query => query
                    .Where(cd => cd.CreditEnabled == null || cd.CreditEnabled == isCompanyCreditEnabled),
                cancellationToken
            );

            foreach (var companyDocument in companyDocuments)
            {
                companyDocument.Files = company.CompanyDocumentsFiles
                    .Where(cdf => cdf.CompanyDocumentId == companyDocument.Id)
                    .Select(cdf => new BaseCompanyDocumentFileDto()
                    {
                        Id = cdf.Id,
                        Name = cdf.File.Name,
                    })
                    .ToList();
            }

            return companyDocuments;
        }
    }
}

using ApiTemplate.Application.Features.Companies.Dtos;
using ApiTemplate.Application.Features.Companies.Profile.Dtos;

namespace ApiTemplate.Application.Features.Companies.Profile;

public class GetCompanyProfileAssociates
{
    public class Request : IRequest<CompanyProfileAssociatesDto>
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

    internal class Handler : IRequestHandler<Request, CompanyProfileAssociatesDto>
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

        public async Task<CompanyProfileAssociatesDto> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.Id,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync<CompanyProfileAssociatesDto>(
                query => query
                    .Select(c => new CompanyProfileAssociatesDto()
                    {
                        Current = new CompanyProfileAssociatesDto.CurrentChange()
                        {
                            HasPepRelative = c.HasPepRelative,
                            UnderOath = c.UnderOath,
                            Associates = c.CompanyAssociates
                                .Where(ca => !ca.IsChange)
                                .Select(ca => new CompanyAssociateDto(
                                    ca.Id,
                                    ca.Name,
                                    ca.DocumentTypeId,
                                    ca.DocumentType.ShortName,
                                    ca.DocumentType.Name,
                                    ca.Document,
                                    ca.ParticipationPercent,
                                    ca.Pep
                                )),
                            AssociatesReviewStatus = c.AssociatesReviewStatus,
                            AssociatesReviewMessage = c.AssociatesReviewMessage,
                        },
                        Change = new CompanyProfileAssociatesDto.CurrentChange()
                        {
                            HasPepRelative = c.CompanyChange!.HasPepRelative,
                            UnderOath = c.CompanyChange.UnderOath,
                            Associates = c.CompanyAssociates
                                .Where(ca => ca.IsChange)
                                .Select(ca => new CompanyAssociateDto(
                                    ca.Id,
                                    ca.Name,
                                    ca.DocumentTypeId,
                                    ca.DocumentType.ShortName,
                                    ca.DocumentType.Name,
                                    ca.Document,
                                    ca.ParticipationPercent,
                                    ca.Pep
                                )),
                            AssociatesReviewStatus = c.CompanyChange.AssociatesReviewStatus,
                            AssociatesReviewMessage = c.CompanyChange.AssociatesReviewMessage,
                        },
                        CompanyInfoReviewStatus = c.CompanyChange.CompanyInfoReviewStatus,
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

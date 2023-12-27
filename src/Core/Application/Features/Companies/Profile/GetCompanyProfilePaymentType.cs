using ApiTemplate.Application.Features.Companies.Profile.Dtos;

namespace ApiTemplate.Application.Features.Companies.Profile;

public class GetCompanyProfilePaymentType
{
    public class Request : IRequest<CompanyProfilePaymentTypeDto>
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

    internal class Handler : IRequestHandler<Request, CompanyProfilePaymentTypeDto>
    {
        private readonly IValidator<Request> _validator;
        private readonly ICurrentUser _currentUser;
        private readonly CompanyService _companyService;

        public Handler(
            IValidator<Request> validator,
            ICurrentUser currentUser,
            CompanyService companyService
        )
        {
            _validator = validator;
            _currentUser = currentUser;
            _companyService = companyService;
        }

        public async Task<CompanyProfilePaymentTypeDto> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.Id,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var documents = await _companyService.GetDocumentsAsync(request.Id, true, cancellationToken);

            return documents;
        }
    }
}

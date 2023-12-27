namespace ApiTemplate.Application.Features.Companies.Onboarding;

public class FinishOnboarding
{
    public class Request : IRequest
    {
        public Guid CompanyId { get; set; }
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
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<CompanyUser> _companyUserRepository;
        private readonly IRepository<Permission> _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly IUtilsService _utilsService;
        private readonly CompanyService _companyService;
        private readonly SignCompanyRegistrationFileService _signCompanyRegistrationFileService;

        public Handler(
            IValidator<Request> validator,
            IRepository<User> userRepository,
            IRepository<Company> companyRepository,
            IRepository<CompanyUser> companyUserRepository,
            IRepository<Permission> permissionsRepository,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser,
            IUtilsService utilsService,
            CompanyService companyService,
            SignCompanyRegistrationFileService signCompanyRegistrationFileService
        )
        {
            _validator = validator;
            _userRepository = userRepository;
            _companyRepository = companyRepository;
            _companyUserRepository = companyUserRepository;
            _permissionRepository = permissionsRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _utilsService = utilsService;
            _companyService = companyService;
            _signCompanyRegistrationFileService = signCompanyRegistrationFileService;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            _currentUser.ValidateCompanyAndPermissionAccess(
                request.CompanyId,
                Common.Constants.Permissions.CompanyProfileAdmin
            );

            var company = await _companyRepository.FirstOrDefaultAsync(query => query
                    .Include(c => c.CompanyChange)
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

            if (company.Status != CompanyStatus.OnBoarding)
            {
                throw new InvalidRequestException(
                    ErrorCodes.CompanyInvalidStatus,
                    $"This company status is '{company.Status.ToString()}'"
                );
            }

            var isCurrentUserSigner = company.PersonType == PersonType.Natural ||
                                      _currentUser.Email == company.LegalRepresentativeEmail;

            if (isCurrentUserSigner)
            {
                var isRegistrationAlreadySigned = company.CompanySignedFiles.Count > 0;
                if (!isRegistrationAlreadySigned)
                {
                    var signedFile = await _signCompanyRegistrationFileService.GenerateAndSignAsync(
                        request.CompanyId,
                        cancellationToken
                    );

                    company.CompanySignedFiles = new[]
                    {
                        new CompanySignedFile()
                        {
                            Id = signedFile.Id,
                            Type = CompanySignedFileType.Registration
                        }
                    };
                }
            }
            else
            {
                company.SignOnboardingToken = _utilsService.GenerateRandomPassword();
            }

            await AttachRemainingPermissionsToCurrentUserAsync(company, cancellationToken);

            var change = company.InitChange();

            change.CompanyInfoReviewStatus = ReviewStatus.Reviewing;
            change.AssociatesReviewStatus = ReviewStatus.Reviewing;
            change.BillingTaxesReviewStatus = ReviewStatus.Reviewing;
            change.DocumentsReviewStatus = ReviewStatus.Reviewing;

            if (change.CreditEnabled == true)
            {
                change.CreditReviewStatus = ReviewStatus.Reviewing;
            }

            company.Status = CompanyStatus.Reviewing;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!isCurrentUserSigner)
            {
                string signerEmail;
                string signerFirstName;
                string signerLastName;

                if (company.PersonType == PersonType.Natural)
                {
                    signerEmail = _currentUser.Email!;
                    signerFirstName = _currentUser.FirstName!;
                    signerLastName = _currentUser.LastName!;
                }
                else
                {
                    signerEmail = company.CompanyChange!.LegalRepresentativeEmail!;
                    signerFirstName = company.CompanyChange.LegalRepresentativeFirstName!;
                    signerLastName = company.CompanyChange.LegalRepresentativeLastName!;
                }

                await _companyService.SendSignRegistrationEmailAsync(
                    company.Id,
                    signerEmail,
                    company.Name,
                    signerFirstName,
                    signerLastName!,
                    company.SignOnboardingToken!,
                    cancellationToken
                );
            }

            return Unit.Value;
        }

        private async Task AttachRemainingPermissionsToCurrentUserAsync(
            Company company,
            CancellationToken cancellationToken
        )
        {
            var companyUser = await _companyUserRepository.FirstOrDefaultAsync(query => query
                    .Include(cu => cu.CompanyUserPermissions)
                    .ThenInclude(cup => cup.Permission)
                    .Where(cu => cu.CompanyId == company.Id && cu.UserId == _currentUser.Id),
                cancellationToken
            );

            if (companyUser == null) return;

            var permissionsToAttach = (company.Type == CompanyType.Customer
                    ? Common.Constants.Permissions.CustomerOnlyPermissions
                    : Common.Constants.Permissions.VendorOnlyPermissions)
                .ToList();

            // remove permissions already attached to the user
            permissionsToAttach.RemoveAll(p =>
                companyUser.CompanyUserPermissions
                    .Select(cup => cup.Permission.Name)
                    .Contains(p)
            );

            var permissionIds = await _permissionRepository.ListAsync<Guid>(query => query
                    .Select(p => p.Id)
                    .Where(p => permissionsToAttach.Contains(p.Name))
                , cancellationToken
            );

            foreach (var permissionId in permissionIds)
            {
                companyUser.CompanyUserPermissions.Add(new CompanyUserPermission
                {
                    PermissionId = permissionId
                });
            }
        }
    }
}

using ApiTemplate.Application.Features.Auth.Dtos;

namespace ApiTemplate.Application.Features.Auth;

public class RegisterCompany
{
    public class Request : IRequest<LoginResponse>
    {
        public string CompanyName { get; set; } = default!;
        public string ContactPhoneNumber { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(p => p.CompanyName)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(p => p.ContactPhoneNumber)
                .NotEmpty()
                .MaximumLength(30);

            RuleFor(p => p.FirstName)
                .NotEmpty()
                .MaximumLength(70);

            RuleFor(p => p.LastName)
                .NotEmpty()
                .MaximumLength(70);

            RuleFor(p => p.Email)
                .NotEmpty()
                .MaximumLength(320)
                .EmailAddress();

            RuleFor(p => p.Password)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(72);
        }
    }

    internal class Handler : IRequestHandler<Request, LoginResponse>
    {
        private readonly IValidator<Request> _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Permission> _permissionRepository;
        private readonly ICookieService _cookieService;
        private readonly ICurrentUser _currentUser;

        public Handler(
            IValidator<Request> validator,
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IRepository<Company> companyRepository,
            IRepository<User> userRepository,
            IRepository<Permission> permissionRepository,
            ICookieService cookieService,
            ICurrentUser currentUser
        )
        {
            _validator = validator;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _companyRepository = companyRepository;
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _cookieService = cookieService;
            _currentUser = currentUser;
        }

        public async Task<LoginResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);

            var isEmailDuplicated = await _userRepository.AnyAsync(
                query => query.Where(u => u.Email == request.Email),
                cancellationToken
            );

            if (isEmailDuplicated)
            {
                throw new InvalidRequestException(
                    ErrorCodes.DuplicatedEmail,
                    $"An user with the email '{request.Email}' already exists"
                );
            }

            var permissionIds = await _permissionRepository.ListAsync<Guid>(query => query
                    .Select(p => p.Id)
                    .Where(p => Common.Constants.Permissions.CustomerVendorPermissions.Contains(p.Name))
                , cancellationToken
            );

            var user = new User()
            {
                Id = Ulid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Email = request.Email.Trim(),
                Enabled = true,
                IsAdmin = false,
            };

            _userRepository.Add(user);

            var companyUser = new CompanyUser()
            {
                Id = Ulid.NewGuid(),
                UserId = user.Id,
                Owner = true,
                CompanyUserPermissions = permissionIds
                    .Select(permissionId => new CompanyUserPermission()
                    {
                        PermissionId = permissionId
                    })
                    .ToList()
            };

            var costCenterUser = new CostCenterUser()
            {
                Id = Ulid.NewGuid(),
                CompanyUserId = companyUser.Id
            };

            var costCenter = new CostCenter()
            {
                Id = Ulid.NewGuid(),
                Name = CostCenter.DefaultName,
                Default = true,
                Enabled = true,
                CostCenterUsers = new[] { costCenterUser }
            };

            var company = new Company()
            {
                Id = Ulid.NewGuid(),
                Name = request.CompanyName,
                ContactPhoneNumber = request.ContactPhoneNumber,
                UsesPurchaseOrder = false,
                CreditEnabled = false,
                Status = CompanyStatus.Approved, // TODO change later to Onboarding when onboarding flow is enabled
                CostCenters = new[] { costCenter },
                CompanyUsers = new[] { companyUser }
            };

            _companyRepository.Add(company);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var tokenResponse = _tokenService.GenerateAuthTokens(user.Id);
            var tokensExpiration = tokenResponse.Adapt<TokensExpirationDto>();

            _cookieService.AttachTokenCookies(tokenResponse.AccessToken, tokenResponse.RefreshToken);
            await _currentUser.SetCurrentAsync(user.Id, user.Email, user.FirstName, user.LastName, cancellationToken);

            var authUser = user.Adapt<AuthUserDto>();

            authUser.Permissions = await _currentUser.GetPermissionsAsync(user.Id, true, cancellationToken);
            authUser.Companies = _currentUser.Companies!;

            return new LoginResponse()
            {
                User = authUser,
                Tokens = tokensExpiration
            };
        }
    }
}

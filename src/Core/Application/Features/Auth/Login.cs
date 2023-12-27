using ApiTemplate.Application.Features.Auth.Dtos;

namespace ApiTemplate.Application.Features.Auth;

public class Login
{
    public class Request : IRequest<LoginResponse>
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(p => p.Email)
                .NotEmpty();

            RuleFor(p => p.Password)
                .NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, LoginResponse>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<LoginAttempt> _loginAttemptRepository;
        private readonly IValidator<Request> _validator;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICookieService _cookieService;
        private readonly ICurrentUser _currentUser;

        public Handler(
            IRepository<User> userUserRepository,
            IRepository<LoginAttempt> loginAttemptRepository,
            IValidator<Request> validator,
            ITokenService tokenService,
            IUnitOfWork unitOfWork,
            ICookieService cookieService,
            ICurrentUser currentUser
        )
        {
            _userRepository = userUserRepository;
            _loginAttemptRepository = loginAttemptRepository;
            _validator = validator;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _cookieService = cookieService;
            _currentUser = currentUser;
        }

        private class AuthUserData : AuthUserDto
        {
            public string? Password { get; set; }
        }

        public async Task<LoginResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var user = await _userRepository.FirstOrDefaultAsync<AuthUserData>(q => q
                    .Where(u => u.Email == request.Email)
                    .Where(u => u.Enabled),
                cancellationToken
            );

            if (user == null)
            {
                await RegisterLoginAttempt(request, null, LoginAttemptStatus.UserNotFound, cancellationToken);

                throw new UnauthorizedException(ErrorCodes.LoginFailed);
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                await RegisterLoginAttempt(request, user.Id, LoginAttemptStatus.IncorrectPassword, cancellationToken);

                throw new UnauthorizedException(ErrorCodes.LoginFailed);
            }

            await RegisterLoginAttempt(request, user.Id, LoginAttemptStatus.Successful, cancellationToken);

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

        private async Task RegisterLoginAttempt(
            Request request,
            Guid? userId,
            LoginAttemptStatus status,
            CancellationToken cancellationToken
        )
        {
            var loginAttempt = new LoginAttempt()
            {
                Id = Ulid.NewGuid(),
                Email = request.Email,
                UserId = userId,
                Status = status,
                CreatedDate = DateTime.UtcNow,
            };

            _loginAttemptRepository.Add(loginAttempt);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

namespace ApiTemplate.Application.Features.Auth;

public class ValidateResetPassword
{
    public class Request : IRequest<UserDto>
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = default!;
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(p => p.Id)
                .NotEmpty();

            RuleFor(p => p.Token)
                .NotEmpty();
        }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string Lastname { get; set; } = default!;
        public string Email { get; set; } = default!;
    }

    internal class Handler : IRequestHandler<Request, UserDto>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IValidator<Request> _validator;

        public Handler(
            IRepository<User> userUserRepository,
            IValidator<Request> validator
        )
        {
            _userRepository = userUserRepository;
            _validator = validator;
        }

        public async Task<UserDto> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var user = await _userRepository.FirstOrDefaultAsync(q => q
                    .Where(u => u.Id == request.Id)
                    .Where(u => u.Enabled),
                cancellationToken
            );

            if (user == null)
            {
                throw new NotFoundException(
                    ErrorCodes.UserNotFound,
                    $"User with id {request.Id} was not found"
                );
            }

            user.ValidateRecoveryCode(request.Token);

            return user.Adapt<UserDto>();
        }
    }
}

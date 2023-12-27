namespace ApiTemplate.Application.Features.Auth;

public class ResetPassword
{
    public class Request : IRequest
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(p => p.Id)
                .NotEmpty();

            RuleFor(p => p.Token)
                .NotEmpty();

            RuleFor(p => p.Password)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(72);
        }
    }

    internal class Handler : IRequestHandler<Request>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<Request> _validator;

        public Handler(
            IValidator<Request> validator,
            IRepository<User> userRepository,
            IUnitOfWork unitOfWork
        )
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
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

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.RecoveryCode = null;
            user.RecoveryExpireDate = null;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

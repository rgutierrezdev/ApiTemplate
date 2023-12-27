using ApiTemplate.Application.Common.Mailing;
using ApiTemplate.Application.Common.Mailing.Templates;

namespace ApiTemplate.Application.Features.Auth;

public class CheckResetPassword
{
    public class Request : IRequest
    {
        public string Email { get; set; } = default!;
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(p => p.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }

    internal class Handler : IRequestHandler<Request>
    {
        private readonly IUtilsService _utilsService;
        private readonly IRepository<User> _userRepository;
        private readonly IValidator<Request> _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailService _mailService;

        public Handler(
            IUtilsService utilsService,
            IRepository<User> userUserRepository,
            IValidator<Request> validator,
            IUnitOfWork unitOfWork,
            IMailService mailService
        )
        {
            _utilsService = utilsService;
            _userRepository = userUserRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _mailService = mailService;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var user = await _userRepository.FirstOrDefaultAsync(q => q
                    .Where(u => u.Email == request.Email)
                    .Where(u => u.Enabled),
                cancellationToken
            );

            if (user == null)
            {
                // TODO log this scenario
                return Unit.Value;
            }

            user.RecoveryCode = _utilsService.GenerateRandomPassword();
            user.RecoveryExpireDate = DateTime.UtcNow.AddHours(1);

            var hashedRecoveryCode = BCrypt.Net.BCrypt.HashPassword(user.RecoveryCode);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _mailService.SendTemplatedEmail(new[] { user.Email }, new ResetPasswordTemplate(
                    new ResetPasswordTemplate.Variables()
                    {
                        Username = user.FirstName,
                        ResetPasswordLink = new AppLink(
                            $"/reset-password?token={Uri.EscapeDataString(hashedRecoveryCode)}&userId={user.Id}"
                        )
                    }),
                cancellationToken
            );

            return Unit.Value;
        }
    }
}

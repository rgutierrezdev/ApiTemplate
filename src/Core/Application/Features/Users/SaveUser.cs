using ApiTemplate.Application.Common.Mailing;
using ApiTemplate.Application.Common.Mailing.Templates;

namespace ApiTemplate.Application.Features.Users;

public class SaveUser
{
    public class Request : IRequest<Response>
    {
        public Guid? Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public bool? Enabled { get; set; }
        public RolesRequest[] Roles { get; set; } = default!;
        public Guid[] RemovedUserRoleIds { get; set; } = default!;
    }

    public record RolesRequest(
        Guid? Id,
        Guid RoleId,
        DateTime? StartDate,
        DateTime? EndDate
    );

    public class Response
    {
        public Guid Id { get; set; }
        public IEnumerable<Guid> UserRoleIds { get; set; } = default!;
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(70);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(70);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(320);

            RuleFor(x => x.Roles)
                .NotNull()
                .ForEach(collection => collection.SetValidator(new RolesRequestValidator()));

            RuleFor(x => x.RemovedUserRoleIds)
                .NotNull();
        }

        public class RolesRequestValidator : AbstractValidator<RolesRequest>
        {
            public RolesRequestValidator()
            {
                RuleFor(x => x.RoleId)
                    .NotEmpty();

                When(x => x.StartDate != null && x.EndDate != null, () =>
                {
                    RuleFor(x => x.StartDate)
                        .LessThanOrEqualTo(x => x.EndDate);
                });
            }
        }
    }

    internal class Handler : IRequestHandler<Request, Response>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilsService _utilsService;
        private readonly IMailService _mailService;

        public Handler(
            IValidator<Request> validator,
            IRepository<User> userRepository,
            IUnitOfWork unitOfWork,
            IUtilsService utilsService,
            IMailService mailService
        )
        {
            _validator = validator;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _utilsService = utilsService;
            _mailService = mailService;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            await ValidateDuplicateEmailAsync(request, cancellationToken);

            var user = await GetUserAsync(request, cancellationToken);

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.Enabled = request.Enabled ?? user.Enabled;
            user.IsAdmin = true;

            var userRoleIds = new List<Guid>();

            foreach (var roleRequest in request.Roles)
            {
                UserRole userRole;

                if (roleRequest.Id == null)
                {
                    userRole = new UserRole()
                    {
                        Id = Ulid.NewGuid(),
                    };

                    user.UserRoles.Add(userRole);
                }
                else
                {
                    userRole = user.UserRoles.First(ur => ur.Id == roleRequest.Id);
                }

                userRoleIds.Add(userRole.Id);

                userRole.UserId = user.Id;
                userRole.RoleId = roleRequest.RoleId;
                userRole.StartDate = roleRequest.StartDate;
                userRole.EndDate = roleRequest.EndDate;
            }

            foreach (var userRoleId in request.RemovedUserRoleIds)
            {
                var userRole = user.UserRoles.FirstOrDefault(ur => ur.Id == userRoleId);

                if (userRole == null) continue;

                user.UserRoles.Remove(userRole);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (request.Id == null)
            {
                await SendInvitationEmailAsync(user, cancellationToken);
            }

            return new Response()
            {
                Id = user.Id,
                UserRoleIds = userRoleIds
            };
        }

        private async Task<User> GetUserAsync(Request request, CancellationToken cancellationToken)
        {
            User user;

            if (request.Id == null)
            {
                user = new User()
                {
                    Id = Ulid.NewGuid(),
                    RecoveryCode = _utilsService.GenerateRandomPassword(),
                    RecoveryExpireDate = DateTime.UtcNow.AddHours(24),
                    UserRoles = new List<UserRole>(),
                    Enabled = true
                };

                _userRepository.Add(user);
            }
            else
            {
                user = await _userRepository.FirstOrDefaultAsync(query => query
                        .Include(u => u.UserRoles)
                        .Where(u => u.Id == request.Id),
                    cancellationToken
                ) ?? throw new NotFoundException(
                    ErrorCodes.UserNotFound,
                    $"User with id '${request.Id}' was not found"
                );
            }

            return user;
        }

        private async Task ValidateDuplicateEmailAsync(Request request, CancellationToken cancellationToken)
        {
            var isEmailDuplicated = await _userRepository.AnyAsync(
                query => query.Where(u => u.Email == request.Email && u.Id != request.Id),
                cancellationToken
            );

            if (isEmailDuplicated)
            {
                throw new InvalidRequestException(
                    ErrorCodes.DuplicatedEmail,
                    $"An user with the email '{request.Email}' already exists"
                );
            }
        }

        private async Task SendInvitationEmailAsync(User user, CancellationToken cancellationToken)
        {
            var hashedRecoveryCode = BCrypt.Net.BCrypt.HashPassword(user.RecoveryCode);

            await _mailService.SendTemplatedEmail(new[] { user.Email }, new AdminUserInvitationTemplate(
                    new AdminUserInvitationTemplate.Variables()
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        NewPasswordLink = new AppLink(
                            $"/reset-password?token={Uri.EscapeDataString(hashedRecoveryCode)}&userId={user.Id}"
                        )
                    }),
                cancellationToken
            );
        }
    }
}

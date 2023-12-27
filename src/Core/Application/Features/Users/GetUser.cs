using ApiTemplate.Application.Features.Users.Dtos;

namespace ApiTemplate.Application.Features.Users;

public class GetUser
{
    public class Request : IRequest<Response>
    {
        public Guid Id { get; }

        public Request(Guid id)
        {
            Id = id;
        }
    }

    public class Response : UserDto
    {
        public IEnumerable<UserRoleDto> UserRoles { get; set; } = default!;
    }

    public record UserRoleDto(
        Guid Id,
        Guid RoleId,
        DateTime? StartDate,
        DateTime? EndDate
    );

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(p => p.Id)
                .NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, Response>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<User> _repository;

        public Handler(IValidator<Request> validator, IRepository<User> repository)
        {
            _validator = validator;
            _repository = repository;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var user = await _repository.FirstOrDefaultAsync<Response>(
                query => query
                    .Select(u => new Response()
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        Enabled = u.Enabled,
                        UserRoles = u.UserRoles.Select(ur => new UserRoleDto(
                            ur.Id,
                            ur.RoleId,
                            ur.StartDate,
                            ur.EndDate
                        )).ToList()
                    })
                    .Where(dt => dt.Id == request.Id),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.UserNotFound,
                $"User with id '${request.Id}' was not found"
            );

            return user;
        }
    }
}

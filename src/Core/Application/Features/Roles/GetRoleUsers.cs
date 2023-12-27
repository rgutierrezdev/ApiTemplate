using ApiTemplate.Application.Features.Roles.Dtos;

namespace ApiTemplate.Application.Features.Roles;

public class GetRoleUsers
{
    public class Request : IRequest<List<RoleUserDto>>
    {
        public Guid Id { get; }

        public Request(Guid id)
        {
            Id = id;
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(p => p.Id)
                .NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, List<RoleUserDto>>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<UserRole> _repository;

        public Handler(IValidator<Request> validator, IRepository<UserRole> repository)
        {
            _validator = validator;
            _repository = repository;
        }

        public async Task<List<RoleUserDto>> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var users = await _repository.ListAsync<RoleUserDto>(
                query => query
                    .Select(ru => new RoleUserDto()
                    {
                        Id = ru.User.Id,
                        FullName = $"{ru.User.FirstName} {ru.User.LastName}",
                    })
                    .Where(ru => ru.RoleId == request.Id),
                cancellationToken
            );

            return users;
        }
    }
}

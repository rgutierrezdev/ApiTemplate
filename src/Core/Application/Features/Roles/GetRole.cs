using ApiTemplate.Application.Features.Roles.Dtos;

namespace ApiTemplate.Application.Features.Roles;

public class GetRole
{
    public class Request : IRequest<RolePermissionsDto>
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

    internal class Handler : IRequestHandler<Request, RolePermissionsDto>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Role> _repository;

        public Handler(IValidator<Request> validator, IRepository<Role> repository)
        {
            _validator = validator;
            _repository = repository;
        }

        public async Task<RolePermissionsDto> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var role = await _repository.FirstOrDefaultAsync<RolePermissionsDto>(
                query => query
                    .Select(r => new RolePermissionsDto()
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Permissions = r.RolePermissions.Select(rp => rp.PermissionId).ToList()
                    })
                    .Where(dt => dt.Id == request.Id),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.RoleNotFound,
                $"Role with id '{request.Id}' was not found"
            );

            return role;
        }
    }
}

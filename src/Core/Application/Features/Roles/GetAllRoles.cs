using ApiTemplate.Application.Features.Roles.Dtos;

namespace ApiTemplate.Application.Features.Roles;

public class GetAllRoles
{
    public class Request : IRequest<List<RoleDto>>
    {
    }

    internal class Handler : IRequestHandler<Request, List<RoleDto>>
    {
        private readonly IRepository<Role> _repository;

        public Handler(IRepository<Role> repository)
        {
            _repository = repository;
        }

        public async Task<List<RoleDto>> Handle(Request request, CancellationToken cancellationToken)
        {
            var roles = await _repository.ListAsync<RoleDto>(query => query
                    .OrderBy(c => c.Name),
                cancellationToken
            );

            // TODO cache this response
            return roles;
        }
    }
}

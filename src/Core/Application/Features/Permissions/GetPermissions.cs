using ApiTemplate.Application.Features.Permissions.Dtos;

namespace ApiTemplate.Application.Features.Permissions;

public class GetPermissions
{
    public class Request : IRequest<List<PermissionDto>>
    {
    }

    internal class Handler : IRequestHandler<Request, List<PermissionDto>>
    {
        private readonly IRepository<Permission> _repository;

        public Handler(IRepository<Permission> repository)
        {
            _repository = repository;
        }

        public Task<List<PermissionDto>> Handle(Request request, CancellationToken cancellationToken)
        {
            return _repository.ListAsync<PermissionDto>(
                query => query
                    .Where(p => p.IsAdmin)
                    .OrderBy(p => p.Name),
                cancellationToken
            );
        }
    }
}

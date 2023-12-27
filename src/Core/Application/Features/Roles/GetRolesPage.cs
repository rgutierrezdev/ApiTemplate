using ApiTemplate.Application.Features.Roles.Dtos;

namespace ApiTemplate.Application.Features.Roles;

public class GetRolesPage
{
    public class Filters
    {
    }

    public enum OrderByFields
    {
        Name
    }

    public class Request : PaginationRequest<Filters, OrderByFields>, IRequest<PaginationResponse<RoleDto>>
    {
    }

    internal class Handler : IRequestHandler<Request, PaginationResponse<RoleDto>>
    {
        private readonly IRepository<Role> _roleRepository;

        public Handler(IRepository<Role> roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<PaginationResponse<RoleDto>> Handle(Request request, CancellationToken cancellationToken)
        {
            var spec = new PaginationSpecification<Role, RoleDto, Filters, OrderByFields>(
                request,
                query =>
                {
                    query.OrderByDescending(r => r.CreatedDate, !request.HasOrderBy());

                    request.HandleFilter(Like.Contains, value => { query.Search(r => r.Name, value); });
                }
            );

            var list = await _roleRepository.ListAsync(spec, cancellationToken);
            var total = await _roleRepository.CountAsync(spec, cancellationToken);

            return new PaginationResponse<RoleDto>(list, total);
        }
    }
}

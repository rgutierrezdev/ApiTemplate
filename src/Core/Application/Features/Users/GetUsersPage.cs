using System.Linq.Expressions;
using System.Runtime.Serialization;
using ApiTemplate.Application.Features.Users.Dtos;

namespace ApiTemplate.Application.Features.Users;

public class GetUsersPage
{
    public class Filters
    {
        public bool? Enabled { get; set; }
        public bool? Invited { get; set; }
        public Guid[]? RoleIds { get; set; }
    }

    public enum OrderByFields
    {
        Name,
        Email
    }

    public class Request : PaginationRequest<Filters, OrderByFields>, IRequest<PaginationResponse<Response>>
    {
    }

    public class Response : UserDto
    {
        public bool Invited { get; set; }
        public string[] Roles { get; set; } = default!;
    }

    internal class Handler : IRequestHandler<Request, PaginationResponse<Response>>
    {
        private readonly IRepository<User> _repository;

        public Handler(IRepository<User> repository)
        {
            _repository = repository;
        }

        public async Task<PaginationResponse<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var spec = new PaginationSpecification<User, Response, Filters, OrderByFields>(
                request,
                query =>
                {
                    query
                        .Select(u => new Response()
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Email = u.Email,
                            Enabled = u.Enabled,
                            Invited = u.Password == "" || u.Password == null,
                            Roles = u.UserRoles
                                .Where(ur =>
                                    (ur.StartDate == null || ur.StartDate <= DateTime.UtcNow) &&
                                    (ur.EndDate == null || ur.EndDate >= DateTime.UtcNow)
                                )
                                .Select(ur => ur.Role.Name)
                                .ToArray()
                        })
                        .Where(u => u.IsAdmin)
                        .OrderByDescending(u => u.CreatedDate, !request.HasOrderBy());

                    request
                        .HandleFilter(Like.Contains, value =>
                        {
                            query
                                .Search(u => u.FirstName + " " + u.LastName, value)
                                .Search(u => u.Email, value);
                        })
                        .HandleFilter(f => f.Enabled, value => { query.Where(u => u.Enabled == value); })
                        .HandleFilter(f => f.Invited, value =>
                        {
                            if (value == true)
                                query.Where(u => u.Password == "" || u.Password == null);
                            else
                                query.Where(u => u.Password != "" && u.Password != null);
                        })
                        .HandleFilter(f => f.RoleIds, value =>
                            query.Where(u => u.UserRoles.Any(ur => value!.Contains(ur.RoleId)))
                        );
                },
                new Dictionary<OrderByFields, Expression<Func<User, object?>>>
                {
                    { OrderByFields.Name, u => u.FirstName + " " + u.LastName }
                }
            );

            var list = await _repository.ListAsync(spec, cancellationToken);
            var total = await _repository.CountAsync(spec, cancellationToken);

            return new PaginationResponse<Response>(list, total);
        }
    }
}

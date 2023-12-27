namespace ApiTemplate.Application.Features.Roles;

public class SaveRole
{
    public class Request : IRequest<Guid>
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = default!;
        public Difference Permissions { get; set; } = default!;
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(50);

            RuleForEach(p => p.Permissions.Added).NotEmpty();

            RuleForEach(p => p.Permissions.Removed).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, Guid>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<RolePermission> _rolePermissionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(
            IValidator<Request> validator,
            IRepository<Role> roleRepository,
            IRepository<RolePermission> rolePermissionRepository,
            IUnitOfWork unitOfWork
        )
        {
            _validator = validator;
            _roleRepository = roleRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            Role role;

            if (request.Id == null)
            {
                role = new Role()
                {
                    Id = Ulid.NewGuid(),
                    Name = request.Name
                };

                _roleRepository.Add(role);
            }
            else
            {
                role = await _roleRepository.FirstOrDefaultAsync(
                    query => query.Where(r => r.Id == request.Id),
                    cancellationToken
                ) ?? throw new NotFoundException(
                    ErrorCodes.RoleNotFound,
                    $"Role with id '{request.Id}' was not found"
                );

                role.Name = request.Name;
            }

            foreach (var permissionId in request.Permissions.Added)
            {
                var rolePermission = new RolePermission()
                {
                    RoleId = role.Id,
                    PermissionId = permissionId,
                };

                _rolePermissionRepository.Add(rolePermission);
            }

            foreach (var permissionId in request.Permissions.Removed)
            {
                var rolePermission = await _rolePermissionRepository.FirstOrDefaultAsync(q => q
                        .Where(rp => rp.RoleId == request.Id)
                        .Where(rp => rp.PermissionId == permissionId),
                    cancellationToken
                );

                if (rolePermission != null)
                {
                    _rolePermissionRepository.Delete(rolePermission);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return role.Id;
        }
    }
}

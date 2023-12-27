namespace ApiTemplate.Application.Features.Roles;

public class DeleteRole
{
    public class Request : IRequest<Guid>
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

    internal class Handler : IRequestHandler<Request, Guid>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<Role> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IValidator<Request> validator, IRepository<Role> repository, IUnitOfWork unitOfWork)
        {
            _validator = validator;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var role = await _repository.FirstOrDefaultAsync(
                query => query
                    .Include(r => r.RolePermissions)
                    .Where(r => r.Id == request.Id),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.RoleNotFound,
                $"Role with id '{request.Id}' was not found"
            );

            _repository.Delete(role);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return request.Id;
        }
    }
}

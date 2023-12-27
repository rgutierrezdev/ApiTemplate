namespace ApiTemplate.Application.Features.Users;

public class DeleteUser
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
        private readonly IRepository<User> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IValidator<Request> validator, IRepository<User> repository, IUnitOfWork unitOfWork)
        {
            _validator = validator;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var user = await _repository.FirstOrDefaultAsync(
                query => query
                    .Include(u => u.UserRoles)
                    .Where(u => u.Id == request.Id),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.UserNotFound,
                $"User with id '${request.Id}' was not found"
            );

            _repository.Delete(user);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return request.Id;
        }
    }
}

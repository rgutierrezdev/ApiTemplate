using ApiTemplate.Application.Features.Auth.Dtos;

namespace ApiTemplate.Application.Features.Auth;

public class Current
{
    public class Request : IRequest<CurrentResponse>
    {
    }

    internal class Handler : IRequestHandler<Request, CurrentResponse>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<User> _userRepository;

        public Handler(
            ICurrentUser currentUser,
            IRepository<User> userUserRepository
        )
        {
            _currentUser = currentUser;
            _userRepository = userUserRepository;
        }

        public async Task<CurrentResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FirstOrDefaultAsync<AuthUserDto>(
                query => query.Where(u => u.Id == _currentUser.Id),
                cancellationToken
            );

            if (user == null)
            {
                throw new NotFoundException(
                    ErrorCodes.UserNotFound,
                    $"User with id {_currentUser.Id} was not found"
                );
            }

            user.Permissions = await _currentUser.GetPermissionsAsync(user.Id, true, cancellationToken);
            user.Companies = _currentUser.Companies!;

            return new CurrentResponse()
            {
                User = user,
            };
        }
    }
}

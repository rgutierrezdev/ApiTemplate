using ApiTemplate.Application.Features.Cities.Dtos;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Application.Features.Cities;

public class GetCity
{
    public class Request : IRequest<CityDto>
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

    internal class Handler : IRequestHandler<Request, CityDto>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<City> _repository;

        public Handler(IValidator<Request> validator, IRepository<City> repository)
        {
            _validator = validator;
            _repository = repository;
        }

        public async Task<CityDto> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var city = await _repository.FirstOrDefaultAsync<CityDto>(
                query => query
                    .Select(c => new CityDto()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        StateId = c.StateId,
                        StateName = c.State.Name,
                        CountryId = c.State.CountryId,
                        CountryName = c.State.Country.Name
                    })
                    .Where(c => c.Id == request.Id),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.CityNotFound,
                $"City with id '{request.Id}' was not found"
            );

            return city;
        }
    }
}

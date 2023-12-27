using ApiTemplate.Application.Features.Countries.Dtos;

namespace ApiTemplate.Application.Features.Countries;

public class GetCountries
{
    public class Request : IRequest<List<CountryDto>>
    {
    }

    internal class Handler : IRequestHandler<Request, List<CountryDto>>
    {
        private readonly IRepository<Country> _repository;

        public Handler(IRepository<Country> repository)
        {
            _repository = repository;
        }

        public async Task<List<CountryDto>> Handle(Request request, CancellationToken cancellationToken)
        {
            var countries = await _repository.ListAsync<CountryDto>(query => query
                    .OrderBy(c => c.Name),
                cancellationToken
            );

            // TODO cache this response
            return countries;
        }
    }
}

using ApiTemplate.Application.Features.Cities.Dtos;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Application.Features.Cities;

public class SearchCities
{
    public class Request : IRequest<List<CityDto>>
    {
        public Guid? CountryId { get; set; }
        public Guid? StateId { get; set; }
        public string? Filter { get; set; } = default!;
    }

    internal class Handler : IRequestHandler<Request, List<CityDto>>
    {
        private readonly IRepository<City> _repository;

        public Handler(IRepository<City> repository)
        {
            _repository = repository;
        }

        public async Task<List<CityDto>> Handle(Request request, CancellationToken cancellationToken)
        {
            var businessStructures = await _repository.ListAsync<CityDto>(query =>
                {
                    if (request.CountryId != null && request.CountryId != Guid.Empty)
                    {
                        query.Where(c => c.State.CountryId == request.CountryId);
                    }

                    if (request.StateId != null && request.StateId != Guid.Empty)
                    {
                        query.Where(c => c.StateId == request.StateId);
                    }

                    if (!string.IsNullOrWhiteSpace(request.Filter))
                    {
                        var filter = $"%{request.Filter.Trim()}%";

                        query.Search(c => c.Name, filter);
                    }

                    query
                        .Select(c => new CityDto()
                        {
                            Id = c.Id,
                            Name = c.Name,
                            StateId = c.StateId,
                            StateName = c.State.Name,
                            CountryId = c.State.CountryId,
                            CountryName = c.State.Country.Name
                        })
                        .Take(50)
                        .OrderBy(c => c.Name);
                },
                cancellationToken
            );

            return businessStructures;
        }
    }
}

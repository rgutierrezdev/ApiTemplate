using ApiTemplate.Application.Features.EconomicSectors.Dtos;

namespace ApiTemplate.Application.Features.EconomicSectors;

public class GetEconomicSectors
{
    public class Request : IRequest<List<EconomicSectorDto>>
    {
    }

    internal class Handler : IRequestHandler<Request, List<EconomicSectorDto>>
    {
        private readonly IRepository<EconomicSector> _repository;

        public Handler(IRepository<EconomicSector> repository)
        {
            _repository = repository;
        }

        public async Task<List<EconomicSectorDto>> Handle(Request request, CancellationToken cancellationToken)
        {
            var economicSectors = await _repository.ListAsync<EconomicSectorDto>(query => query
                    .OrderBy(dt => dt.Name),
                cancellationToken
            );

            // TODO cache this response
            return economicSectors;
        }
    }
}

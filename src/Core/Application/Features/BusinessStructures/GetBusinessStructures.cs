using ApiTemplate.Application.Features.BusinessStructures.Dtos;

namespace ApiTemplate.Application.Features.BusinessStructures;

public class GetBusinessStructures
{
    public class Request : IRequest<List<BusinessStructureDto>>
    {
    }

    internal class Handler : IRequestHandler<Request, List<BusinessStructureDto>>
    {
        private readonly IRepository<BusinessStructure> _repository;

        public Handler(IRepository<BusinessStructure> repository)
        {
            _repository = repository;
        }

        public async Task<List<BusinessStructureDto>> Handle(Request request, CancellationToken cancellationToken)
        {
            var businessStructures = await _repository.ListAsync<BusinessStructureDto>(query => query
                    .OrderBy(bs => bs.Name),
                cancellationToken
            );

            // TODO cache this response
            return businessStructures;
        }
    }
}

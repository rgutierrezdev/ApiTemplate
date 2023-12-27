using ApiTemplate.Application.Features.DocumentTypes.Dtos;

namespace ApiTemplate.Application.Features.DocumentTypes;

public class GetDocumentTypes
{
    public class Request : IRequest<List<DocumentTypeDto>>
    {
    }

    internal class Handler : IRequestHandler<Request, List<DocumentTypeDto>>
    {
        private readonly IRepository<DocumentType> _repository;

        public Handler(IRepository<DocumentType> repository)
        {
            _repository = repository;
        }

        public async Task<List<DocumentTypeDto>> Handle(Request request, CancellationToken cancellationToken)
        {
            var documentTypes = await _repository.ListAsync<DocumentTypeDto>(query => query
                    .OrderBy(dt => dt.Name),
                cancellationToken
            );

            // TODO cache this response
            return documentTypes;
        }
    }
}

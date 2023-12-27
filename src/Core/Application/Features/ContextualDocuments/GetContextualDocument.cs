using ApiTemplate.Application.Features.ContextualDocuments.Dtos;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Application.Features.ContextualDocuments;

public class GetContextualDocument
{
    public class Request : IRequest<ContextualDocumentDto>
    {
        public ContextualDocumentType Type { get; }

        public Request(ContextualDocumentType type)
        {
            Type = type;
        }
    }

    internal class Handler : IRequestHandler<Request, ContextualDocumentDto>
    {
        private readonly IRepository<ContextualDocument> _repository;

        public Handler(IRepository<ContextualDocument> repository)
        {
            _repository = repository;
        }

        public async Task<ContextualDocumentDto> Handle(Request request, CancellationToken cancellationToken)
        {
            var document = await _repository.FirstOrDefaultAsync<ContextualDocumentDto>(
                query => query
                    .Where(cd => cd.Type == request.Type)
                    .OrderByDescending(cd => cd.Version),
                cancellationToken
            ) ?? throw new NotFoundException(
                ErrorCodes.ContextualDocumentNotFound,
                $"Contextual Document with type '{request.Type.ToString()}' was not found"
            );

            return document;
        }
    }
}
